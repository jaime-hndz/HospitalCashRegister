using HospitalCashRegister.Data;
using HospitalCashRegister.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitalCashRegister.Controllers
{
    public class CashRegistersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CashRegistersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(
                await _context.CashRegisters
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
        public async Task<IActionResult> Start(CashRegister obj)
        {
            if (ModelState.IsValid)
            {
                var id = Guid.NewGuid().ToString();
                obj.Id = id;

                HttpContext.Session.SetString("CurrentCashRegisterId", id);
                _context.Add(obj);
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

            var obj = await _context.CashRegisters.FindAsync(id);
            if (obj == null)
            {
                return NotFound();
            }

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

        public async Task<IActionResult> End()
        {
            var obj = await _context.CashRegisters
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
        public async Task<IActionResult> EndConfirmed(string id)
        {
            var obj = await _context.CashRegisters.FindAsync(id);
            if (obj == null) throw new Exception("Este registro no existe");
            obj.CashRegisterStatusId = 1;
            obj.ClosingDate = DateTime.Now;
            obj.FinalAmount = obj.InitialAmount + obj.CashInflow - obj.CashOutflow;
            _context.CashRegisters.Update(obj);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("CurrentCashRegisterId");
            return RedirectToAction(nameof(Index));
        }

        private bool EntityExists(string id)
        {
            return _context.CashRegisters.Any(e => e.Id == id);
        }


    }
}
