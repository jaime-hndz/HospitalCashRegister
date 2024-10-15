using HospitalCashRegister.Data;
using HospitalCashRegister.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitalCashRegister.Controllers
{
    public class CashiersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CashiersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(
                await _context.Cashiers
                    .Where(x => x.Status == true)
                    .Include(x => x.Branch)
                    .ToListAsync()
            );
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = await _context.Cashiers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        public IActionResult Create()
        {

            var branches = _context.Branches.ToList();
            ViewBag.Branches = new SelectList(branches, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cashier obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Admin == true)
                {
                    obj.BranchId = null;
                }
                obj.Id = Guid.NewGuid().ToString();
                obj.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(obj.Password,11,BCrypt.Net.HashType.SHA256);

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

            var obj = await _context.Cashiers.FindAsync(id);
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

            var obj = await _context.Cashiers
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
            var obj = await _context.Cashiers.FindAsync(id);
            if (obj == null) throw new Exception("Este registro no existe");
            obj.Status = false;
            _context.Cashiers.Update(obj);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EntityExists(string id)
        {
            return _context.Cashiers.Any(e => e.Id == id);
        }
    }
}
