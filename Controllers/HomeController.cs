using BloodCare.Data;
using BloodCare.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloodCare.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // angka=kapasitas
            const int kapasitasAcuan = 150;

            var golonganList = new[] { "A", "B", "AB", "O" };

            var stokPerGolongan = await _context.StokDarahs
                .GroupBy(s => s.GolonganDarah)
                .Select(g => new { Golongan = g.Key, Total = g.Sum(s => s.JumlahKantong) })
                .ToDictionaryAsync(x => x.Golongan, x => x.Total);

            var stokList = golonganList.Select(g =>
            {
                int jumlah = stokPerGolongan.TryGetValue(g, out var total) ? total : 0;
                int persen = kapasitasAcuan > 0 ? (int)Math.Min(100, jumlah * 100.0 / kapasitasAcuan) : 0;

                return new StokRingkasViewModel
                {
                    Golongan = g,
                    JumlahKantong = jumlah,
                    Persen = persen
                };
            }).ToList();

            var vm = new LandingPageViewModel
            {
                StokPerGolongan = stokList,
                TotalPendonorAktif = await _context.Pendonors.CountAsync(p => p.Status == "Aktif"),
                TotalKantongTersalurkan = await _context.RiwayatDonors.CountAsync(),
                TotalJadwalAktif = await _context.JadwalDonors.CountAsync(j => j.Tanggal.Date >= DateTime.Today)
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
