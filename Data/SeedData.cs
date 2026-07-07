using BloodCare.Models;
using Microsoft.AspNetCore.Identity;

namespace BloodCare.Data
{
    public static class SeedData
    {
        // Dipanggil sekali saat aplikasi start (lihat Program.cs)
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Buat role kalau belum ada
            string[] roles = { "Admin", "Petugas", "Pendonor" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Buat akun Admin default kalau belum ada
            string adminEmail = "admin@bloodcare.com";
            string adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // 3. (Opsional) Buat akun Petugas default kalau belum ada
            string petugasEmail = "petugas@bloodcare.com";
            string petugasPassword = "Petugas@123";

            var petugasUser = await userManager.FindByEmailAsync(petugasEmail);
            if (petugasUser == null)
            {
                petugasUser = new ApplicationUser
                {
                    UserName = petugasEmail,
                    Email = petugasEmail,
                    FullName = "Petugas PMI",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(petugasUser, petugasPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(petugasUser, "Petugas");
                }
            }
            //    Buat akun Pendonor demo kalau belum ada, lengkap dengan
            //    data master Pendonor yang terhubung (IdPendonor) supaya semua fitur
            //    Pendonor (Jadwal Saya, Riwayat Donor Saya) langsung bisa dites.
            string pendonorEmail = "pendonor@bloodcare.com";
            string pendonorPassword = "Pendonor@123";

            var pendonorUser = await userManager.FindByEmailAsync(pendonorEmail);
            if (pendonorUser == null)
            {
                var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

                var pendonorData = new Pendonor
                {
                    NamaLengkap = "Budi Santoso",
                    Umur = 28,
                    JenisKelamin = "Laki-laki",
                    GolonganDarah = "O",
                    Rhesus = "+",
                    Alamat = "Jl. Contoh No. 1, Jakarta",
                    NoHp = "081234567890",
                    Email = pendonorEmail,
                    TanggalDaftar = DateTime.Now,
                    Status = "Aktif"
                };
                context.Pendonors.Add(pendonorData);
                await context.SaveChangesAsync();

                pendonorUser = new ApplicationUser
                {
                    UserName = pendonorEmail,
                    Email = pendonorEmail,
                    FullName = "Budi Santoso",
                    EmailConfirmed = true,
                    IdPendonor = pendonorData.Id
                };

                var result = await userManager.CreateAsync(pendonorUser, pendonorPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(pendonorUser, "Pendonor");
                }
            }
        }
    }
}
