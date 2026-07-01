using BloodCare.Data;
using BloodCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloodCare.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Hitung total pendonor
            ViewBag.TotalPendonor = await _context.Pendonors.CountAsync();

            // Hitung total jadwal donor
            ViewBag.TotalJadwal = await _context.JadwalDonors.CountAsync();

            // Hitung permintaan darah pending
            ViewBag.TotalPermintaanPending = await _context.PermintaanDarahs
                .Where(p => p.Status == "Pending")
                .CountAsync();

            // Stok darah per golongan (untuk kartu ringkasan)
            ViewBag.StokDarah = await _context.StokDarahs
                .GroupBy(s => s.GolonganDarah)
                .Select(g => new
                {
                    GolonganDarah = g.Key,
                    TotalKantong = g.Sum(s => s.JumlahKantong)
                })
                .ToListAsync();

            // 5 jadwal donor terdekat
            ViewBag.JadwalTerdekat = await _context.JadwalDonors
                .Where(j => j.Tanggal >= DateTime.Today)
                .OrderBy(j => j.Tanggal)
                .Take(5)
                .ToListAsync();

            // 5 permintaan darah terbaru
            ViewBag.PermintaanTerbaru = await _context.PermintaanDarahs
                .OrderByDescending(p => p.TanggalPermintaan)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }
}
