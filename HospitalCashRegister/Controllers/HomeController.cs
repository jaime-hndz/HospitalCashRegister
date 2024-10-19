using HospitalCashRegister.Data;
using HospitalCashRegister.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HospitalCashRegister.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context,ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            CheckCashRegister();
            ViewBag.Username = User.Identity?.Name;
            ViewBag.FullName = User.FindFirst("FullName")?.Value;
            ViewBag.BranchName = User.FindFirst("BranchName")?.Value;
            ViewBag.CashRegisterId = HttpContext.Session.GetString("CurrentCashRegisterId");
            ViewBag.Transactions = _context.Transactions.ToList();
            return View();

        }

        private void CheckCashRegister()
        {
            var branchId = User.FindFirst("BranchId")?.Value;

            if (!string.IsNullOrWhiteSpace(branchId))
            {
                var lastCashRegister = _context.CashRegisters
                                            .Where(x => x.BranchId == branchId)
                                            .OrderByDescending(x => x.OpeningDate)
                                            .FirstOrDefault();
                if (lastCashRegister != null && lastCashRegister.CashRegisterStatusId == 0 )
                {
                   HttpContext.Session.SetString("CurrentCashRegisterId", lastCashRegister.Id);
                }
            }


        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}