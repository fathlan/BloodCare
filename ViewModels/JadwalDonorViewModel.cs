using BloodCare.Models;

namespace BloodCare.ViewModels
{
    public class JadwalDonorViewModel
    {
        public JadwalDonor Jadwal { get; set; } = null!;

        // Jumlah orang yang sudah mendaftar ke jadwal ini (dari tabel PendaftaranJadwals)
        public int JumlahTerdaftar { get; set; }

        // Khusus untuk Pendonor yang sedang login: apakah dia sudah daftar ke jadwal ini
        public bool SudahDaftar { get; set; }
    }
}
