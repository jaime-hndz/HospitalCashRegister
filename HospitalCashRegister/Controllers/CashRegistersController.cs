using HospitalCashRegister.Data;
using HospitalCashRegister.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

namespace HospitalCashRegister.Controllers
{
    public class CashRegistersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CashRegistersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> PrintCashRegister(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = await _context.CashRegisters
                .Include(x => x.Cashier)
                .Include(x => x.Branch)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            var transactions = await _context.Transactions
                .Where(x => x.CashRegisterId == obj.Id)
                .Include(x => x.MedicalService)
                .Include(x => x.Patient)
                .ToListAsync();

            obj.transactions = transactions;
            return new ViewAsPdf("PrintCashRegister", obj)
            {
                FileName = $"PrintCashRegister {id}.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }

        public async Task<IActionResult> Index()
        {
            var branchId = User.FindFirst("BranchId")?.Value;

            if (!string.IsNullOrEmpty(branchId))
            {
                ViewBag.CashRegisterId = HttpContext.Session.GetString("CurrentCashRegisterId");
                ViewBag.BranchId = branchId;
                return View(
                    await _context.CashRegisters
                        .Where(cr => cr.BranchId == branchId)
                        .Include(x => x.Branch)
                        .OrderByDescending(x => x.OpeningDate)
                        .ToListAsync()
                );

            }
            else
            {
                return View(
                    await _context.CashRegisters
                        .Include(x => x.Branch)
                        .OrderByDescending(x => x.OpeningDate)
                        .ToListAsync()
                );
            }
        }

        public async Task<IActionResult> GetByBranchId(string branchId)
        {

                return View(
                    await _context.CashRegisters
                        .Where(x => x.BranchId == branchId)
                        .OrderByDescending(x => x.OpeningDate)
                        .ToListAsync()
                );
            
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = await _context.CashRegisters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        public IActionResult Start()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(CashRegister obj, CancellationToken cancellationToken)
        {
            var branchId = User.FindFirst("BranchId")?.Value;
            var cashierId = User.FindFirst("UserId")?.Value;

            if (branchId == null || cashierId == null)
                throw new Exception();

            if (ModelState.IsValid)
            {
                var id = Guid.NewGuid().ToString();
                obj.Id = id;
                obj.BranchId = branchId;
                obj.CashierId = cashierId;

                HttpContext.Session.SetString("CurrentCashRegisterId", id);
                _context.Add(obj);
                await _context.SaveChangesAsync(cancellationToken);
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

            var obj = await _context.CashRegisters.FindAsync(id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Cashier obj, CancellationToken cancellationToken)
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
                    await _context.SaveChangesAsync(cancellationToken);
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

        public async Task<IActionResult> End()
        {
            var branchId = User.FindFirst("BranchId")?.Value;

            if (branchId == null)
            {
                return NotFound();
            }
            var obj = await _context.CashRegisters
                .Where(x => x.BranchId == branchId)
                .OrderByDescending(x => x.OpeningDate)
                .FirstOrDefaultAsync();
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost, ActionName("End")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EndConfirmed(string id, CancellationToken cancellationToken)
        {
            var obj = await _context.CashRegisters.FindAsync(id);
            if (obj == null) throw new Exception("Este registro no existe");
            obj.CashRegisterStatusId = CashRegisterStatus.Closed;
            obj.ClosingDate = DateTime.Now;
            obj.FinalAmount = obj.InitialAmount + obj.CashInflow - obj.CashOutflow;
            _context.CashRegisters.Update(obj);
            await _context.SaveChangesAsync(cancellationToken);
            HttpContext.Session.Remove("CurrentCashRegisterId");

            return RedirectToAction("EndToPrint", new { Id = id });
        }

        public IActionResult EndToPrint(string id)
        {
            ViewBag.Id = id;
            return View();
        }

        private bool EntityExists(string id)
        {
            return _context.CashRegisters.Any(e => e.Id == id);
        }


    }
}
