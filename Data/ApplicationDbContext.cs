using BloodCare.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BloodCare.Data
{
    // PENTING: ganti dari "DbContext" menjadi "IdentityDbContext<ApplicationUser>"
    // Ini otomatis menambah tabel AspNetUsers, AspNetRoles, AspNetUserRoles, dll
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pendonor> Pendonors { get; set; }
        public DbSet<StokDarah> StokDarahs { get; set; }
        public DbSet<PermintaanDarah> PermintaanDarahs { get; set; }
        public DbSet<JadwalDonor> JadwalDonors { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<RiwayatDonor> RiwayatDonors { get; set; }
        public DbSet<PendaftaranJadwal> PendaftaranJadwals { get; set; }
        public DbSet<NotifikasiDibaca> NotifikasiDibacas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // WAJIB dipanggil untuk Identity
        }
    }
}
