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

        public AuthenticationController(ApplicationDbContext context)
        {
            _context = context;
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
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }

            var cashier = await _context.Cashiers
                .FirstOrDefaultAsync(u => u.Username == username);

            if (cashier == null || !VerifyPassword(cashier, password))
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }

            // Aquí puedes implementar tu lógica de autenticación
            // Por ejemplo, usando Claims para manejar sesiones o cookies
            await SignInUser(cashier);

            return RedirectToAction("Index", "Home");
        }

        private bool VerifyPassword(Cashier user, string password)
        {
            // Verificar que el hash de la contraseña coincide
            // Suponiendo que PasswordHash es un hash almacenado de la contraseña
            return true;
        }

        private async Task SignInUser(Cashier cashier)
        {
            // Implementar la lógica de inicio de sesión
            // Usar ClaimsPrincipal para autenticar al usuario

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, cashier.Username),
            new Claim(ClaimTypes.Role, "User")
        };

            var identity = new ClaimsIdentity(claims, "Login");
            var principal = new ClaimsPrincipal(identity);

            // Usar el servicio de autenticación para establecer la cookie de sesión
            await HttpContext.SignInAsync(principal);
        }
    }
}
