using Microsoft.AspNetCore.Identity;

namespace BloodCare.Models
{
    // ApplicationUser = akun login (Admin, Petugas, Pendonor)
    // Untuk User Umum tidak perlu akun, karena mereka tidak login
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        // Opsional: kalau mau hubungkan akun Pendonor ke data Pendonor
        public int? IdPendonor { get; set; }
    }
}
