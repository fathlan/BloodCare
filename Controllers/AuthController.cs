using BloodCare.Data;
using BloodCare.Models;
using BloodCare.Services;
using BloodCare.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BloodCare.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditService _auditService;
        private readonly ApplicationDbContext _context;

        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IAuditService auditService,
            ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _auditService = auditService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Dashboard");

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                await _auditService.LogAsync(user?.Id, user?.UserName, "Login", "Auth", user?.Id);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Dashboard");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty,
                    "Akun terkunci karena terlalu banyak percobaan login gagal. Coba lagi beberapa menit lagi.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email atau password salah.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Dashboard");

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
            {
                ModelState.AddModelError(string.Empty, "Email sudah terdaftar. Gunakan email lain.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Semua yang daftar lewat form publik otomatis jadi "Pendonor"
                await _userManager.AddToRoleAsync(user, "Pendonor");

                // Buat juga data master Pendonor (UC001: "data pendukung lainnya")
                // dan hubungkan ke akun lewat ApplicationUser.IdPendonor supaya
                // fitur "Riwayat Donor Saya" & "Mendaftar Jadwal Donor" bisa jalan.
                var pendonor = new Pendonor
                {
                    NamaLengkap = model.FullName,
                    Umur = model.Umur,
                    JenisKelamin = model.JenisKelamin,
                    GolonganDarah = model.GolonganDarah,
                    Rhesus = model.Rhesus,
                    Alamat = model.Alamat,
                    NoHp = model.NoHp,
                    Email = model.Email,
                    TanggalLahir = model.TanggalLahir,
                    TanggalDaftar = DateTime.Now,
                    Status = "Aktif"
                };
                _context.Pendonors.Add(pendonor);
                await _context.SaveChangesAsync();

                user.IdPendonor = pendonor.Id;
                user.PhoneNumber = model.NoHp;
                await _userManager.UpdateAsync(user);

                await _auditService.LogAsync(user.Id, user.UserName, "Register", "AspNetUsers", user.Id);

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Dashboard");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await _auditService.LogAsync(user.Id, user.UserName, "Logout", "Auth", user.Id);
            }

            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
