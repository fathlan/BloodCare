using BloodCare.Models;
using BloodCare.Services;
using BloodCare.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloodCare.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuditService _auditService;

        public UserManagementController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAuditService auditService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _auditService = auditService;
        }

        // GET: UserManagement
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<UserListViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserListViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName ?? "-",
                    Email = user.Email ?? "-",
                    Role = roles.FirstOrDefault() ?? "Tidak ada role",
                    IsActive = user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.Now,
                    TanggalDaftar = DateTime.Now
                });
            }

            // Summary untuk mini stats
            ViewBag.TotalUser    = userList.Count;
            ViewBag.TotalAdmin   = userList.Count(u => u.Role == "Admin");
            ViewBag.TotalPetugas = userList.Count(u => u.Role == "Petugas");
            ViewBag.TotalPendonor= userList.Count(u => u.Role == "Pendonor");

            return View(userList);
        }

        // GET: UserManagement/Detail/id
        public async Task<IActionResult> Detail(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name!).ToList();

            var vm = new UserDetailViewModel
            {
                Id = user.Id,
                FullName = user.FullName ?? "-",
                Email = user.Email ?? "-",
                Role = roles.FirstOrDefault() ?? "Tidak ada role",
                IsActive = user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.Now,
                PhoneNumber = user.PhoneNumber,
                AvailableRoles = allRoles
            };

            return View(vm);
        }

        // GET: UserManagement/Edit/id
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name!).ToList();

            var vm = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty,
                IsActive = user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.Now,
                AvailableRoles = allRoles
            };

            return View(vm);
        }

        // POST: UserManagement/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel vm)
        {
            var user = await _userManager.FindByIdAsync(vm.Id);
            if (user == null) return NotFound();

            // Update nama
            user.FullName = vm.FullName;

            // Update status aktif
            if (vm.IsActive)
                user.LockoutEnd = null;
            else
                user.LockoutEnd = DateTimeOffset.MaxValue;

            await _userManager.UpdateAsync(user);

            // Update role
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, vm.Role);

            // Audit
            var currentUser = await _userManager.GetUserAsync(User);
            await _auditService.LogAsync(
                currentUser?.Id,
                currentUser?.UserName,
                "UpdateUser",
                "AspNetUsers",
                user.Id,
                $"Role lama: {string.Join(",", currentRoles)} -> Role baru: {vm.Role}, IsActive: {vm.IsActive}"
            );

            TempData["SweetSuccess"] = $"User {user.Email} berhasil diperbarui!";
            return RedirectToAction(nameof(Index));
        }

        // POST: UserManagement/ToggleActive
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            bool isCurrentlyActive = user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.Now;

            if (isCurrentlyActive)
                user.LockoutEnd = DateTimeOffset.MaxValue; // nonaktifkan
            else
                user.LockoutEnd = null; // aktifkan

            await _userManager.UpdateAsync(user);

            // Audit
            var currentUser = await _userManager.GetUserAsync(User);
            await _auditService.LogAsync(
                currentUser?.Id,
                currentUser?.UserName,
                isCurrentlyActive ? "DeactivateUser" : "ActivateUser",
                "AspNetUsers",
                user.Id
            );

            TempData["SweetSuccess"] = $"Status user {user.Email} berhasil diubah!";
            return RedirectToAction(nameof(Index));
        }

        // POST: UserManagement/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Reset ke password default
            string newPassword = "Reset@123";
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                await _auditService.LogAsync(
                    currentUser?.Id,
                    currentUser?.UserName,
                    "ResetPassword",
                    "AspNetUsers",
                    user.Id
                );

                TempData["SweetSuccess"] = $"Password {user.Email} direset ke: {newPassword}";
            }
            else
            {
                TempData["SweetError"] = "Gagal reset password.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: UserManagement/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Jangan hapus akun sendiri
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == id)
            {
                TempData["SweetError"] = "Tidak bisa menghapus akun sendiri!";
                return RedirectToAction(nameof(Index));
            }

            await _auditService.LogAsync(
                currentUser?.Id,
                currentUser?.UserName,
                "DeleteUser",
                "AspNetUsers",
                user.Id,
                $"Email: {user.Email}"
            );

            await _userManager.DeleteAsync(user);
            TempData["SweetSuccess"] = $"User {user.Email} berhasil dihapus!";
            return RedirectToAction(nameof(Index));
        }
    }
}
