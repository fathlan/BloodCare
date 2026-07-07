using BloodCare.Data;
using BloodCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloodCare.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Auth");

            var roles = await _userManager.GetRolesAsync(user);

            // No. HP idealnya ada di data master Pendonor (diisi saat registrasi/diubah Admin).
            // PhoneNumber di Identity dipakai sebagai fallback kalau akun tidak terhubung ke Pendonor
            // (mis. akun Admin/Petugas yang dibuat manual dan tidak punya baris Pendonor).
            string? noHp = user.PhoneNumber;
            if (user.IdPendonor.HasValue)
            {
                var pendonor = await _context.Pendonors.FindAsync(user.IdPendonor.Value);
                if (pendonor != null && !string.IsNullOrWhiteSpace(pendonor.NoHp))
                    noHp = pendonor.NoHp;
            }

            ViewBag.FullName = user.FullName ?? user.Email ?? "User";
            ViewBag.Email = user.Email;
            ViewBag.PhoneNumber = noHp;
            ViewBag.Role = roles.FirstOrDefault() ?? "-";
            ViewBag.RoleLabel = roles.Contains("Admin") ? "Superadmin"
                              : roles.Contains("Petugas") ? "Petugas PMI"
                              : "Pendonor";

            return View();
        }
    }
}
