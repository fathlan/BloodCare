using System;

namespace BloodCare.Models
{
    public class PermintaanDarah
    {
        public int Id { get; set; }
        public string NamaPasien { get; set; }
        public string RumahSakit { get; set; }
        public string GolonganDarah { get; set; }
        public string Rhesus { get; set; }
        public int JumlahKebutuhan { get; set; }
        public string Status { get; set; }
        public DateTime TanggalPermintaan { get; set; }
    }
}