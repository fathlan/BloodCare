using BloodCare.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodCare.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pendonor> Pendonors { get; set; }
        public DbSet<StokDarah> StokDarahs { get; set; }
        public DbSet<PermintaanDarah> PermintaanDarahs { get; set; }
        public DbSet<JadwalDonor> JadwalDonors { get; set; }
    }
}