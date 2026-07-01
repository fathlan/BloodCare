using System;

namespace BloodCare.Models
{
    // Mencatat setiap aksi penting: siapa, ngapain, kapan, terhadap data apa
    public class AuditLog
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public string? UserName { get; set; }

        // Contoh: Login, Logout, Create, Update, Delete, Register
        public string Action { get; set; } = string.Empty;

        // Nama tabel/entity yang terkena aksi, contoh: "PermintaanDarah"
        public string TableName { get; set; } = string.Empty;

        // Id data yang terkena aksi (kalau ada)
        public string? RecordId { get; set; }

        // Data sebelum diubah (opsional, dalam bentuk teks/JSON)
        public string? OldValues { get; set; }

        // Data setelah diubah (opsional, dalam bentuk teks/JSON)
        public string? NewValues { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
