using BCrypt.Net;
using HospitalCashRegister.Data;
using HospitalCashRegister.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HospitalCashRegister.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(ApplicationDbContext context, ILogger<AuthenticationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Debe ingresar todos los datos");
                return View();
            }

            var cashier = await _context.Cashiers
                .Include(c => c.Branch)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (cashier == null || !VerifyPassword(cashier, password))
            {
                ModelState.AddModelError(string.Empty, "Datos erróneos");
                return View();
            }

            cashier.LastSeen = DateTime.Now;

            _context.Cashiers.Update(cashier);
            await _context.SaveChangesAsync();

            // Aquí puedes implementar tu lógica de autenticación
            // Por ejemplo, usando Claims para manejar sesiones o cookies
            await SignInUser(cashier);

            return RedirectToAction("Index", "Home");
        }

        private bool VerifyPassword(Cashier user, string password)
        {
            bool passwordMatches = BCrypt.Net.BCrypt.Verify(password, user.Password) ||
                                   BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password,
                                       HashType.SHA256);

            return passwordMatches;
        }

        private async Task SignInUser(Cashier cashier)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, cashier.Username),
                new Claim("UserId", cashier.Id),
                new Claim(ClaimTypes.Role,  cashier.Admin ? "Admin":"User"),
                new Claim("FullName", cashier.FullName),
                new Claim("BranchId", cashier.BranchId != null ? cashier.BranchId : ""),
                new Claim("BranchName", cashier.Branch != null ? cashier.Branch.Name : "")

            };

            var identity = new ClaimsIdentity(claims, "Login");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Authentication");
        }
    }
}
