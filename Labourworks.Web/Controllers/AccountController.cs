
using Labourworks.Application.DTOs;
using Labourworks.Domain.Entities;
using Labourworks.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Labourworks.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AccountController(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = model.UserID.Trim();

            var user = await _context.UserMaster
                .FirstOrDefaultAsync(u =>
                    u.UserID == userId &&
                    u.DeletedDate == null);

            if (user == null ||
                !user.IsActive ||
                user.Status != "Active" ||
                string.IsNullOrWhiteSpace(user.Password) ||
                user.Password != model.Password)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid User ID or password.");

                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserMasterId.ToString()),

                new Claim(ClaimTypes.Name, user.UserID),

                new Claim("FullName", user.FullName),

                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                });

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult InitialPasswordSetup()
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            return View(new InitialPasswordSetupDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InitialPasswordSetup(
            InitialPasswordSetupDto model)
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var userId = model.UserID.Trim();

            var user = await _context.UserMaster
                .FirstOrDefaultAsync(u =>
                    u.UserID == userId &&
                    u.DeletedDate == null);

            if (user == null)
            {
                ModelState.AddModelError(
                    nameof(model.UserID),
                    "User not found.");

                return View(model);
            }

            if (!user.IsActive || user.Status != "Active")
            {
                ModelState.AddModelError(
                    string.Empty,
                    "This user is not active.");

                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "A password is already set for this user.");

                return View(model);
            }

            user.Password = model.NewPassword;

            await _context.SaveChangesAsync();

            return View("InitialPasswordSetupSuccess");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
