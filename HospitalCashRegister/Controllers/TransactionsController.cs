using Flurl.Http;
using HospitalCashRegister.Data;
using HospitalCashRegister.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using NuGet.Common;
using Rotativa.AspNetCore;

namespace HospitalCashRegister.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TransactionsController> _logger;
        private readonly IConfiguration _configuration;

        public TransactionsController(ApplicationDbContext context, ILogger<TransactionsController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        private async Task<List<MedicalService>> GetServices()
        {
            var baseUrl = _configuration.GetValue<string>("integration:Endpoint");
            string token = _configuration.GetValue<string>("integration:Token");
            string requestUrl = $"{baseUrl}/Servicios/GetServicios";

            var result = await requestUrl
                .PostJsonAsync(token);

            var obj = JsonConvert.DeserializeObject<List<dynamic>>(await result.GetStringAsync());

            if (obj == null)
                throw new Exception("Error al solicitar los servicios");

            var servicios = new List<MedicalService>();

            foreach (var item in obj)
            {
                servicios.Add(new MedicalService
                {
                    Id = item.id,
                    Price = item.costo,
                    Name = item.tipoServicio.descripcion + " " + item.areasMedicas.descripcion,
                    Description = item.tipoServicio.descripcion + " " + item.areasMedicas.descripcion,
                    Status = item.estado
                });
            }

            return servicios;
        }

        private async Task<List<Patient>> GetPatients()
        {
            var baseUrl = _configuration.GetValue<string>("integration:Endpoint");
            string token = _configuration.GetValue<string>("integration:Token");
            string requestUrl = $"{baseUrl}/Usuario/GetUsuarioWIthCita";

            var result = await requestUrl
                .PostJsonAsync(token);

            var obj = JsonConvert.DeserializeObject<List<dynamic>>(await result.GetStringAsync());

            if (obj == null)
                throw new Exception("Error al solicitar los pacientes");

            var pacientes = new List<Patient>();

            foreach (var item in obj)
            {
                pacientes.Add(new Patient
                {
                    Id = item.id,
                    Name = item.name + " " + item.lastName,
                    Document = item.cedula,
                    Address = item.address
                    
                });
            }

            return pacientes;
        }

        private async Task<bool> CheckServices(CancellationToken cancellationToken)
        {

            try
            {
                var coreServices = await GetServices();
                var localServices = _context.MedicalServices.ToList();


                foreach (var service in coreServices)
                {
                    var existingService = localServices.FirstOrDefault(s => s.Id == service.Id);

                    if (existingService != null)
                    {
                        if (!PropertiesDiff<MedicalService>(existingService, service)) continue;
                        _context.Entry(existingService).CurrentValues.SetValues(service);
                        _context.Update(existingService);
                    }
                    else
                    {
                        await _context.MedicalServices.AddAsync(service, cancellationToken);
                    }
                }

/*                var listDelete = localServices.Where(x => !coreServices.Any(y => x.Id == y.Id));

                foreach (var service in listDelete)
                {
                    service.Status = false;
                }
                _context.MedicalServices.UpdateRange(listDelete);*/

                await _context.SaveChangesAsync(cancellationToken);
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

        }

        private async Task<bool> CheckPatients(CancellationToken cancellationToken)
        {

            try
            {
                var corePatients = await GetPatients();
                var localPatients = _context.Patients.ToList();


                foreach (var patient in corePatients)
                {
                    var existing = localPatients.FirstOrDefault(s => s.Id == patient.Id);

                    if (existing != null)
                    {
                        if (!PropertiesDiff<Patient>(existing, patient)) continue;
                        _context.Entry(existing).CurrentValues.SetValues(patient);
                        _context.Update(existing);
                    }
                    else
                    {
                        await _context.Patients.AddAsync(patient,cancellationToken);
                    }
                }

                var unSyncPatients = localPatients.Where(x => !corePatients.Any(y => x.Document == y.Document)).ToList();

                if (unSyncPatients != null)
                {
                    await PostPatients(unSyncPatients);
                }


                await _context.SaveChangesAsync(cancellationToken);
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

        }

        public async Task CheckTransactions(CancellationToken cancellationToken)
        {
            try
            {
                var unappliedTransactions = _context.Transactions
                    .Where(x => x.TransactionStatusId == TransctionStatus.pending)
                    .ToList();

                foreach (var item in unappliedTransactions)
                {
                    item.TransactionStatusId = await PostTransactions(item)
                        ? TransctionStatus.applied
                        : TransctionStatus.pending;
                }

                _context.Transactions.UpdateRange(unappliedTransactions);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
        }

        public async Task<bool> PostPatients(List<Patient> patients)
        {
            try
            {
                var baseUrl = _configuration.GetValue<string>("integration:Endpoint");
                string token = _configuration.GetValue<string>("integration:Token");
                string requestUrl = $"{baseUrl}/Usuario/AddUsuario";

                foreach (var item in patients)
                {
                    string[] names = item.Name.Split(' ');

                    var result = await requestUrl
                    .PostJsonAsync(new
                    {
                        id = item.Id,
                        userName = item.Id,
                        email = item.Id,
                        name = names[0],
                        lastName = "",
                        phoneNumber = item.PhoneNumber1,
                        address = item.Address,
                        birthday = DateAndTime.Now,
                        cedula = item.Document,
                        password = "Abc123*",
                        roleName = "Usuario",
                        token = token
                    });

                }


                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

        }

        private static bool PropertiesDiff<T>(T entidad1, T entidad2)
        {
            var propiedades = typeof(T).GetProperties();

            foreach (var propiedad in propiedades)
            {
                object? valorEntidad1 = propiedad.GetValue(entidad1);
                object? valorEntidad2 = propiedad.GetValue(entidad2);

                if (!object.Equals(valorEntidad1, valorEntidad2))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> PostTransactions(Transaction transaction)
        {

            try
            {

                var baseUrl = _configuration.GetValue<string>("integration:Endpoint");
                string token = _configuration.GetValue<string>("integration:Token");
                string requestUrl = $"{baseUrl}/Transaccion/AddTransaccion";

                var result = await requestUrl
                    .PostJsonAsync(new
                    {
                        idCajero = transaction.CashierId,
                        idPaciente =  transaction.PatientId,
                        idTipoTransaccion = (int)transaction.TransactionTypeId + 1,
                        idEstadoTransaccion = (int)transaction.TransactionStatusId + 1,
                        monto = transaction.Amount,
                        fecha = transaction.Date,
                        comentario = transaction.Comment ?? "string",
                        token = token
                    });

                var obj = JsonConvert.DeserializeObject<dynamic>(await result.GetStringAsync());

                if (obj == null)
                    throw new Exception("Error al solicitar los pacientes");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            try
            {
                await CheckTransactions(cancellationToken);

                var CashRegisterId = HttpContext.Session.GetString("CurrentCashRegisterId");

                if (CashRegisterId != null)
                {
                    ViewBag.CashRegisterId = CashRegisterId;
                    return View(
                    await _context.Transactions
                        .Where(t => t.CashRegisterId == CashRegisterId)
                        .Include(x => x.Cashier)
                        .OrderByDescending(x => x.Date)
                        .ToListAsync()
                        );
                }
                else
                {
                    if (User.IsInRole("Admin"))
                    {
                        return View(
                        await _context.Transactions
                            .Include(x => x.Cashier)
                            .Include(x => x.ServiceOrders)
                            .OrderByDescending(x => x.Date)
                            .ToListAsync()
                        );
                    }
                    else
                    {
                        return View();

                    }
                }

            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
            
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Transaction? obj;
            bool isPayment = ( _context.Transactions.FirstOrDefault(x => x.Id == id).TransactionTypeId  == TransactionType.ServicePayment);

            if (isPayment)
            {
                obj = await _context.Transactions
                 .Include(x => x.Cashier)
                 .Include(x => x.Patient)
                 .Include(x => x.ServiceOrders)
                     .ThenInclude(s => s.MedicalService)
                 .FirstOrDefaultAsync(m => m.Id == id);
            }
            else
            {
               obj = await _context.Transactions
                 .Include(x => x.Cashier)
                 .FirstOrDefaultAsync(m => m.Id == id);
            }
            
            if (obj == null)
            {
                return NotFound();
            }

            if (obj.CashDetails != null)
            {
                 ViewBag.CashDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(obj.CashDetails);
            }
            return View(obj);
        }

        public async Task<IActionResult> PrintTransaction(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = await _context.Transactions
                .Include(x => x.Cashier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            return new ViewAsPdf("PrintTransaction", obj)
            {
                FileName = $"Transccion {id}.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }

        public async Task<IActionResult> PrintInvoice(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = await _context.Transactions
                .Include(x => x.Cashier)
                .Include(x => x.Patient)
                .Include(x => x.ServiceOrders)
                    .ThenInclude(s => s.MedicalService)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            return new ViewAsPdf("PrintInvoice", obj)
            {
                FileName = $"Transccion {id}.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> CreatePayment(CancellationToken cancellationToken)
        {
            var syncServices = await CheckServices(cancellationToken);
            var syncPatients = await CheckPatients(cancellationToken);

            if (!syncServices || !syncPatients)
                _logger.LogError("No se sincronizaron los datos correctamente");

            var patiens = _context.Patients.Where(x => x.Status == true).ToList();
            ViewBag.Patients = new SelectList(patiens, "Id", "Name");

            var services = _context.MedicalServices.Where(x => x.Status == true).ToList();
            ViewBag.MedicalServices = new SelectList(services, "Id", "Name");
            ViewBag.ServicePrices = services.Select(x => new {x.Id, x.Price }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transaction obj)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var cashregisterId = HttpContext.Session.GetString("CurrentCashRegisterId");

            if (userId == null || cashregisterId == null)
                throw new Exception();

            if (ModelState.IsValid)
            {
                obj.Id = Guid.NewGuid().ToString();
                obj.CashierId = userId;
                obj.CashRegisterId = cashregisterId;

                _context.Add(obj);

                var currentCashRegister = _context.CashRegisters.FirstOrDefault(x => x.Id == cashregisterId);
                
                if(currentCashRegister == null)
                    throw new Exception();

                if (obj.TransactionTypeId == TransactionType.CashInflow)
                {
                    currentCashRegister.CashInflow = currentCashRegister.CashInflow + obj.Amount;
                }
                else
                {
                    currentCashRegister.CashOutflow = currentCashRegister.CashOutflow + obj.Amount;
                }


                obj.TransactionStatusId = await PostTransactions(obj) ? TransctionStatus.applied : TransctionStatus.pending;

                _context.CashRegisters.Update(currentCashRegister);

                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePayment(Transaction obj)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var cashregisterId = HttpContext.Session.GetString("CurrentCashRegisterId");
            var services = new List<MedicalService>();


            foreach (var serviceId in obj.ServiceIds)
            {
                services.Add(_context.MedicalServices.FirstOrDefault(x => x.Id == serviceId));
            }

            if (userId == null || cashregisterId == null)
                throw new Exception();

            if (ModelState.IsValid)
            {
                obj.Id = Guid.NewGuid().ToString();
                obj.CashierId = userId;
                obj.CashRegisterId = cashregisterId;
                obj.Amount = services.Sum(s => s.Price);
                obj.TransactionTypeId = TransactionType.ServicePayment;

                obj.TransactionStatusId = await PostTransactions(obj) ? TransctionStatus.applied : TransctionStatus.pending;

                _context.Add(obj);

                _context.ServiceOrders.AddRange(services.Select(s => new ServiceOrder
                {
                    Id = Guid.NewGuid().ToString(),
                    ServiceId = s.Id,
                    TransactionId = obj.Id
                }));

                var currentCashRegister = _context.CashRegisters.FirstOrDefault(x => x.Id == cashregisterId);

                if (currentCashRegister == null)
                    throw new Exception();

                currentCashRegister.CashInflow = currentCashRegister.CashInflow + obj.Amount;
                _context.CashRegisters.Update(currentCashRegister);


                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = await _context.Transactions.FindAsync(id);
            if (obj == null)
            {
                return NotFound();
            }
            var branches = _context.Branches.ToList();
            ViewBag.Branches = new SelectList(branches, "Id", "Name");
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Cashier obj)
        {
            if (id != obj.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    obj.Modified = DateTime.Now;
                    _context.Update(obj);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntityExists(obj.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = await _context.Transactions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var currentTransaction = _context.Transactions.Include(x => x.CashRegister).FirstOrDefault(x => x.Id == id);

            var currentCashRegister = currentTransaction?.CashRegister;

            if (currentCashRegister == null)
                throw new Exception("Error con el cash register");

            if (currentCashRegister.CashRegisterStatusId == CashRegisterStatus.Open)
            {
                currentTransaction.TransactionStatusId = TransctionStatus.rollback;
                if (currentTransaction.TransactionTypeId == TransactionType.CashOutflow)
                {
                    currentCashRegister.CashOutflow = currentCashRegister.CashOutflow - currentTransaction.Amount;

                }
                else
                {
                    currentCashRegister.CashInflow = currentCashRegister.CashInflow - currentTransaction.Amount;
                }


            }

             _context.Transactions.Update(currentTransaction);
             _context.CashRegisters.Update(currentCashRegister);
            await _context.SaveChangesAsync();  

            return RedirectToAction(nameof(Index));
        }

        public IActionResult CashDetailsTable(string cashDeatilsRaw)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(cashDeatilsRaw);
            return View(data); 
        }

        private bool EntityExists(string id)
        {
            return _context.Cashiers.Any(e => e.Id == id);
        }
    }
}
