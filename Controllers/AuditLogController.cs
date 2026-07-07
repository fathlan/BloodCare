using BloodCare.Data;
using BloodCare.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloodCare.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AuditLog
        public async Task<IActionResult> Index(AuditLogFilterViewModel filter)
        {
            var query = _context.AuditLogs.AsQueryable();

            // Filter by username
            if (!string.IsNullOrEmpty(filter.SearchUser))
                query = query.Where(a => a.UserName != null &&
                    a.UserName.Contains(filter.SearchUser));

            // Filter by action
            if (!string.IsNullOrEmpty(filter.FilterAction))
                query = query.Where(a => a.Action == filter.FilterAction);

            // Filter by table
            if (!string.IsNullOrEmpty(filter.FilterTable))
                query = query.Where(a => a.TableName == filter.FilterTable);

            // Filter by tanggal
            if (filter.TanggalDari.HasValue)
                query = query.Where(a => a.Timestamp >= filter.TanggalDari.Value);

            if (filter.TanggalSampai.HasValue)
                query = query.Where(a => a.Timestamp <= filter.TanggalSampai.Value.AddDays(1));

            var logs = await query
                .OrderByDescending(a => a.Timestamp)
                .Take(200) // batasi 200 record terbaru
                .ToListAsync();

            // Data untuk dropdown filter
            ViewBag.Actions = await _context.AuditLogs
                .Select(a => a.Action)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();

            ViewBag.Tables = await _context.AuditLogs
                .Select(a => a.TableName)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();

            // Summary stats
            ViewBag.TotalLog     = await _context.AuditLogs.CountAsync();
            ViewBag.TotalHariIni = await _context.AuditLogs
                .CountAsync(a => a.Timestamp.Date == DateTime.Today);
            ViewBag.TotalLogin   = await _context.AuditLogs
                .CountAsync(a => a.Action == "Login");
            ViewBag.TotalDelete  = await _context.AuditLogs
                .CountAsync(a => a.Action == "Delete" || a.Action == "DeleteUser");

            ViewBag.Filter = filter;

            return View(logs);
        }

        // GET: AuditLog/Detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var log = await _context.AuditLogs.FindAsync(id);
            if (log == null) return NotFound();
            return View(log);
        }

        // POST: AuditLog/ClearOld — hapus log lebih dari 30 hari
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearOld()
        {
            var batas = DateTime.Now.AddDays(-30);
            var oldLogs = _context.AuditLogs.Where(a => a.Timestamp < batas);
            _context.AuditLogs.RemoveRange(oldLogs);
            await _context.SaveChangesAsync();

            TempData["SweetSuccess"] = "Log lebih dari 30 hari berhasil dihapus!";
            return RedirectToAction(nameof(Index));
        }
    }
}
