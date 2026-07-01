using BloodCare.Data;
using BloodCare.Models;

namespace BloodCare.Services
{
    public interface IAuditService
    {
        Task LogAsync(string? userId, string? userName, string action, string tableName,
            string? recordId = null, string? oldValues = null, string? newValues = null);
    }

    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;

        public AuditService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string? userId, string? userName, string action, string tableName,
            string? recordId = null, string? oldValues = null, string? newValues = null)
        {
            var log = new AuditLog
            {
                UserId = userId,
                UserName = userName,
                Action = action,
                TableName = tableName,
                RecordId = recordId,
                OldValues = oldValues,
                NewValues = newValues,
                Timestamp = DateTime.Now
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
