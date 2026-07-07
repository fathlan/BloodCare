using BloodCare.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloodCare.Controllers
{
    // UC010/UC011: Melihat dan Membuat Laporan (Admin)
    [Authorize(Roles = "Admin")]
    public class LaporanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LaporanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Laporan
        public async Task<IActionResult> Index(DateTime? dari, DateTime? sampai)
        {
            var mulai = dari ?? DateTime.Today.AddDays(-30);
            var akhir = sampai ?? DateTime.Today;

            ViewBag.Dari = mulai;
            ViewBag.Sampai = akhir;

            //Ringkasan Riwayat Donor per periode
            var riwayatPeriode = await _context.RiwayatDonors
                .Where(r => r.TanggalDonor.Date >= mulai.Date && r.TanggalDonor.Date <= akhir.Date)
                .ToListAsync();

            ViewBag.TotalDonorPeriode = riwayatPeriode.Count;
            ViewBag.TotalMlPeriode = riwayatPeriode.Sum(r => r.JumlahDarahMl);

            ViewBag.DonorPerGolongan = riwayatPeriode
                .GroupBy(r => r.GolonganDarah + r.Rhesus)
                .Select(g => new { Golongan = g.Key, Jumlah = g.Count() })
                .OrderByDescending(g => g.Jumlah)
                .ToList();

            // Ringkasan Pendonor
            ViewBag.TotalPendonor = await _context.Pendonors.CountAsync();
            ViewBag.PendonorBaruPeriode = await _context.Pendonors
                .CountAsync(p => p.TanggalDaftar.Date >= mulai.Date && p.TanggalDaftar.Date <= akhir.Date);

            //  Ringkasan Stok 
            ViewBag.StokPerGolongan = await _context.StokDarahs
                .OrderBy(s => s.GolonganDarah).ThenBy(s => s.Rhesus)
                .ToListAsync();
            ViewBag.TotalStok = await _context.StokDarahs.SumAsync(s => s.JumlahKantong);

            // Ringkasan Jadwal
            ViewBag.TotalJadwalPeriode = await _context.JadwalDonors
                .CountAsync(j => j.Tanggal.Date >= mulai.Date && j.Tanggal.Date <= akhir.Date);

            //Ringkasan Permintaan Darah
            ViewBag.TotalPermintaanPeriode = await _context.PermintaanDarahs
                .CountAsync(p => p.TanggalPermintaan.Date >= mulai.Date && p.TanggalPermintaan.Date <= akhir.Date);
            ViewBag.PermintaanTerpenuhi = await _context.PermintaanDarahs
                .CountAsync(p => p.TanggalPermintaan.Date >= mulai.Date && p.TanggalPermintaan.Date <= akhir.Date && p.Status == "Terpenuhi");

            return View();
        }

        // GET: Laporan CSV sederhana
        public async Task<IActionResult> Export(DateTime? dari, DateTime? sampai)
        {
            var mulai = dari ?? DateTime.Today.AddDays(-30);
            var akhir = sampai ?? DateTime.Today;

            var riwayat = await _context.RiwayatDonors
                .Where(r => r.TanggalDonor.Date >= mulai.Date && r.TanggalDonor.Date <= akhir.Date)
                .OrderBy(r => r.TanggalDonor)
                .ToListAsync();

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Tanggal,Nama Pendonor,Lokasi,Golongan Darah,Jumlah (ml),Status");
            foreach (var r in riwayat)
            {
                csv.AppendLine($"{r.TanggalDonor:yyyy-MM-dd},{r.NamaPendonor},{r.LokasiDonor},{r.GolonganDarah}{r.Rhesus},{r.JumlahDarahMl},{r.Status}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"Laporan_Donor_{mulai:yyyyMMdd}_{akhir:yyyyMMdd}.csv");
        }
    }
}
