using BloodCare.Data;
using BloodCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloodCare.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            //dicek dulu supaya bisa cabang per-role
            var user = await _userManager.GetUserAsync(User);
            var roles = user != null ? await _userManager.GetRolesAsync(user) : new List<string>();
            ViewBag.NamaUser = user?.FullName ?? user?.Email ?? "User";
            ViewBag.RoleUser = roles.FirstOrDefault() ?? "User";
            ViewBag.InitialUser = (user?.FullName ?? user?.Email ?? "U")
                .Split(' ')
                .Take(2)
                .Aggregate("", (a, b) => a + b.Substring(0, 1).ToUpper());

            // dashboard petugas
            if (roles.Contains("Petugas") && !roles.Contains("Admin"))
            {
                return await IndexPetugas();
            }

            // dashoard pendonor
            if (roles.Contains("Pendonor") && !roles.Contains("Admin") && !roles.Contains("Petugas"))
            {
                return await IndexPendonor(user);
            }

            // stat admin
            ViewBag.TotalPendonor = await _context.Pendonors.CountAsync();

            ViewBag.TotalStok = await _context.StokDarahs
                .SumAsync(s => s.JumlahKantong);

            ViewBag.TotalPermintaan = await _context.PermintaanDarahs.CountAsync();

            ViewBag.PermintaanMenunggu = await _context.PermintaanDarahs
                .CountAsync(p => p.Status == "Pending");

            ViewBag.TotalJadwal = await _context.JadwalDonors.CountAsync();

            // Notifikasi = permintaan darurat (jumlah >= 5 kantong dan pending)
            ViewBag.TotalNotifikasi = await _context.PermintaanDarahs
                .CountAsync(p => p.JumlahKebutuhan >= 5 && p.Status == "Pending");
            ViewBag.NotifikasiMendesak = await _context.PermintaanDarahs
                .CountAsync(p => p.JumlahKebutuhan >= 3 && p.Status == "Pending");

            // === STOK PER GOLONGAN ===
            var stokPerGolongan = await _context.StokDarahs
                .GroupBy(s => s.GolonganDarah)
                .Select(g => new { GolonganDarah = g.Key, Total = g.Sum(s => s.JumlahKantong) })
                .ToListAsync();
            ViewBag.StokPerGolongan = stokPerGolongan;

            int totalStok = stokPerGolongan.Sum(s => s.Total);
            ViewBag.TotalStokKeseluruhan = totalStok;

            // Cek stok golongan O (warning jika < 20)
            var stokO = stokPerGolongan.FirstOrDefault(s => s.GolonganDarah == "O");
            ViewBag.StokOKritis = stokO != null && stokO.Total < 20;

            // gabungan dari permintaan darah
            var aktivitas = await _context.PermintaanDarahs
                .OrderByDescending(p => p.TanggalPermintaan)
                .Take(5)
                .Select(p => new {
                    Nama = p.NamaPasien,
                    Aktivitas = "Permintaan Darah (Gol " + p.GolonganDarah + p.Rhesus + ")",
                    Tanggal = p.TanggalPermintaan,
                    Status = p.Status,
                    RumahSakit = p.RumahSakit
                })
                .ToListAsync();
            ViewBag.Aktivitas = aktivitas;

            //JADWAL TERDEKAT
            ViewBag.JadwalTerdekat = await _context.JadwalDonors
                .Where(j => j.Tanggal >= DateTime.Today)
                .OrderBy(j => j.Tanggal)
                .Take(3)
                .ToListAsync();

            return View();
        }

 
        // DASHBOARD PETUGAS
        private async Task<IActionResult> IndexPetugas()
        {
            var today = DateTime.Today;

            // --- Kegiatan Donor Hari Ini: jadwal yang tanggalnya == hari ini ---
            var kegiatanHariIni = await _context.JadwalDonors
                .Where(j => j.Tanggal.Date == today)
                .OrderBy(j => j.Tanggal)
                .ToListAsync();

            // Kalau tidak ada kegiatan persis hari ini, tampilkan jadwal terdekat
            // supaya panel tidak kosong total buat petugas.
            var kegiatanUntukTabel = kegiatanHariIni.Any()
                ? kegiatanHariIni
                : await _context.JadwalDonors
                    .Where(j => j.Tanggal >= today)
                    .OrderBy(j => j.Tanggal)
                    .Take(4)
                    .ToListAsync();

            ViewBag.JumlahLokasiAktif = kegiatanHariIni.Select(j => j.Lokasi).Distinct().Count();
            ViewBag.KegiatanHariIni = kegiatanUntukTabel;
            ViewBag.MenampilkanJadwalTerdekat = !kegiatanHariIni.Any();

            // Total Riwayat Diinput (bulan ini)
            ViewBag.TotalRiwayatBulanIni = await _context.RiwayatDonors
                .CountAsync(r => r.TanggalDonor.Month == today.Month && r.TanggalDonor.Year == today.Year);

            //Stok Darah Terkini
            ViewBag.TotalStokTerkini = await _context.StokDarahs.SumAsync(s => s.JumlahKantong);

            //Pemberitahuan Sistem: stok kritis + jadwal mendatang (3 hari)
            var pemberitahuan = new List<dynamic>();

            var stokKritis = await _context.StokDarahs
                .Where(s => s.JumlahKantong < 10)
                .OrderBy(s => s.JumlahKantong)
                .Take(3)
                .ToListAsync();
            foreach (var s in stokKritis)
            {
                pemberitahuan.Add(new
                {
                    Judul = $"Stok Darah {s.GolonganDarah} Rhesus {(s.Rhesus == "+" ? "Positif" : "Negatif")} Menipis",
                    Pesan = $"Stok darah {s.GolonganDarah}{s.Rhesus} di bawah batas aman ({s.JumlahKantong} kantong).",
                    Waktu = s.TerakhirDiperbarui,
                    Urgent = s.JumlahKantong < 5
                });
            }

            var jadwalDekat = await _context.JadwalDonors
                .Where(j => j.Tanggal >= today && j.Tanggal <= today.AddDays(3))
                .OrderBy(j => j.Tanggal)
                .Take(2)
                .ToListAsync();
            foreach (var j in jadwalDekat)
            {
                pemberitahuan.Add(new
                {
                    Judul = "Jadwal Donor Akan Datang",
                    Pesan = $"Kegiatan di {j.Lokasi} pada {j.Tanggal:dd MMMM yyyy}. Kuota: {j.Kuota} orang.",
                    Waktu = (DateTime)j.Tanggal,
                    Urgent = false
                });
            }

            ViewBag.PemberitahuanSistem = pemberitahuan
                .OrderByDescending(p => (bool)p.Urgent)
                .ThenByDescending(p => (DateTime)p.Waktu)
                .Take(4)
                .ToList();

            return View("IndexPetugas");
        }

        // DASHBOARD PENDONOR (ringkas: jadwal saya + riwayat donor saya)
        private async Task<IActionResult> IndexPendonor(ApplicationUser? user)
        {
            var idPendonor = user?.IdPendonor;

            ViewBag.TotalDonorSaya = idPendonor.HasValue
                ? await _context.RiwayatDonors.CountAsync(r => r.PendonorId == idPendonor.Value)
                : 0;

            DateTime? donorTerakhir = null;
            if (idPendonor.HasValue)
            {
                donorTerakhir = await _context.Pendonors
                    .Where(p => p.Id == idPendonor.Value)
                    .Select(p => p.DonorTerakhir)
                    .FirstOrDefaultAsync();
            }
            ViewBag.DonorTerakhir = donorTerakhir;

            var jadwalSaya = new List<JadwalDonor>();
            if (user != null)
            {
                jadwalSaya = await (from p in _context.PendaftaranJadwals
                                    join j in _context.JadwalDonors on p.JadwalDonorId equals j.Id
                                    where p.PendonorUserId == user.Id && j.Tanggal >= DateTime.Today
                                    orderby j.Tanggal
                                    select j).Take(3).ToListAsync();
            }
            ViewBag.JadwalSayaMendatang = jadwalSaya;

            ViewBag.JadwalTerdekatUmum = await _context.JadwalDonors
                .Where(j => j.Tanggal >= DateTime.Today)
                .OrderBy(j => j.Tanggal)
                .Take(3)
                .ToListAsync();

            return View("IndexPendonor");
        }
    }
}
