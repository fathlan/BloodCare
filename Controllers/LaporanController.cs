using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloodCare.Models;
using BloodCare.Data;

namespace BloodCare.Controllers
{
    public class LaporanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LaporanController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.RiwayatDonors.AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(r => r.TanggalDonor >= startDate && r.TanggalDonor <= endDate);
                ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
                ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            }

            var laporanData = await query.ToListAsync();
            return View(laporanData);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateLaporan(string keterangan)
        {
            var laporanBaru = new Laporan
            {
                AdminId = 1, // Hardcode id_admin sementara
                JenisLaporan = "Laporan Donor Masuk",
                TanggalLaporan = DateTime.Now,
                Keterangan = keterangan
            };

            _context.Laporans.Add(laporanBaru);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Log Laporan berhasil diarsip ke database!";
            return RedirectToAction(nameof(Index));
        }
    }
}