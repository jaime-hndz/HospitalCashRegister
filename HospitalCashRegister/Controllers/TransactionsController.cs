using HospitalCashRegister.Data;
using HospitalCashRegister.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

namespace HospitalCashRegister.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var CashRegisterId = HttpContext.Session.GetString("CurrentCashRegisterId");

                if (CashRegisterId != null)
                {
                    ViewBag.CashRegisterId = CashRegisterId;
                    return View(
                    await _context.Transactions
                        .Where(t => t.CashRegisterId == CashRegisterId)
                        .Include(x => x.Cashier)
                        .Include(x => x.Patient)
                        .Include(x => x.MedicalService)
                        .Include(x => x.ServiceOrders)
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
                            .Include(x => x.Patient)
                            .Include(x => x.MedicalService)
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

            var obj = await _context.Transactions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obj == null)
            {
                return NotFound();
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
                .Include(x => x.Patient)
                .Include(x => x.ServiceOrders)
                    .ThenInclude(s => s.MedicalService)
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

        public IActionResult Create()
        {

            var patiens = _context.Patients.ToList();
            ViewBag.Patients = new SelectList(patiens, "Id", "Name");

            var services = _context.MedicalServices.ToList();
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

                _context.Add(obj);

                _context.ServiceOrders.AddRange(services.Select(s => new ServiceOrder
                {
                    Id = Guid.NewGuid().ToString(),
                    ServiceId = s.Id,
                    TransactionId = obj.Id
                }));

                var currentCashRegister = _context.CashRegisters.FirstOrDefault(x => x.Id == cashregisterId);
                
                if(currentCashRegister == null)
                    throw new Exception();

                if (obj.TransactionTypeId == TransactionType.CashInflow)
                {
                    currentCashRegister.CashInflow += obj.Amount;
                }
                else
                {
                    currentCashRegister.CashOutflow += obj.Amount;
                }

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
/*            var obj = await _context.Cashiers.FindAsync(id);
            if (obj == null) throw new Exception("Este registro no existe");
            obj.Status = false;
            _context.Cashiers.Update(obj);
            await _context.SaveChangesAsync();*/
            return RedirectToAction(nameof(Index));
        }

        private bool EntityExists(string id)
        {
            return _context.Cashiers.Any(e => e.Id == id);
        }
    }
}
