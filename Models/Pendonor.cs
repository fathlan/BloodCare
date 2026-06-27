using System;

namespace BloodCare.Models
{
    public class Pendonor
    {
        public int Id { get; set; }
        public string Nama { get; set; }
        public int Umur { get; set; }
        public string JenisKelamin { get; set; }
        public string GolonganDarah { get; set; }
        public string Rhesus { get; set; }
        public string Alamat { get; set; }
        public string NoHP { get; set; }
        public DateTime TanggalDonorTerakhir { get; set; }
    }
}