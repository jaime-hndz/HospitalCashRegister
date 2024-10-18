using HospitalCashRegister.Data;
using HospitalCashRegister.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitalCashRegister.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(
                await _context.Patients
                    .Where(x => x.Status == true)
                    .ToListAsync()
            );
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = await _context.Patients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient obj, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                obj.Id = Guid.NewGuid().ToString();

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

            var obj = await _context.Patients.FindAsync(id);
            if (obj == null)
            {
                return NotFound();
            }
            var branches = _context.Patients.ToList();
            ViewBag.Branches = new SelectList(branches, "Id", "Name");
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Patient obj, CancellationToken cancellationToken)
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

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = await _context.Patients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id, CancellationToken cancellationToken)
        {
            var obj = await _context.Patients.FindAsync(id);
            if (obj == null) throw new Exception("Este registro no existe");
            obj.Status = false;
            _context.Patients.Update(obj);
            await _context.SaveChangesAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        private bool EntityExists(string id)
        {
            return _context.Cashiers.Any(e => e.Id == id);
        }
    }
}
