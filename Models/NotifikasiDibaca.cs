using System.ComponentModel.DataAnnotations;

namespace BloodCare.Models
{
    // Notifikasi di sistem ini sintetis (dihasilkan otomatis dari StokDarah/
    // PermintaanDarah/JadwalDonor, bukan tabel notifikasi sendiri). Tabel ini
    // cuma mencatat notifikasi sintetis mana (via NotifId, misal "stok-5")
    // yang sudah dibaca oleh user mana, supaya status baca/belum bisa persisten.
    public class NotifikasiDibaca
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        // Id sintetis dari NotifikasiController, contoh: "stok-5", "permintaan-12", "jadwal-3"
        [Required]
        public string NotifId { get; set; } = string.Empty;

        public DateTime WaktuDibaca { get; set; } = DateTime.Now;
    }
}
