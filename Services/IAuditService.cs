namespace BloodCare.Services
{
    // Kontrak service audit trail. Dipakai oleh AuthController, PendonorsController,
    // StokDarahController, UserManagementController, dan controller baru (RiwayatDonor,
    // PendaftaranJadwal, Laporan) untuk mencatat siapa melakukan apa.
    public interface IAuditService
    {
        Task LogAsync(
            string? userId,
            string? userName,
            string action,
            string tableName,
            string? recordId,
            string? note = null);
    }
}
