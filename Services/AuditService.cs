using BloodCare.Data;
using BloodCare.Models;

namespace BloodCare.Services
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;

        public AuditService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(
            string? userId,
            string? userName,
            string action,
            string tableName,
            string? recordId,
            string? note = null)
        {
            var log = new AuditLog
            {
                UserId = userId,
                UserName = userName,
                Action = action,
                TableName = tableName,
                RecordId = recordId,
                NewValues = note,
                Timestamp = DateTime.Now
            };

            _context.AuditLogs.Add(log);

            // Audit trail tidak boleh menggagalkan aksi utama pengguna kalau gagal disimpan.
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Sengaja ditelan: mencatat log gagal bukan alasan untuk membatalkan
                // operasi utama (mis. simpan riwayat donor) yang sudah terjadi.
            }
        }
    }
}
