using System;

namespace BloodCare.Models
{
    public class JadwalDonor
    {
        public int Id { get; set; }
        public string Lokasi { get; set; }
        public DateTime Tanggal { get; set; }
        public int Kuota { get; set; }
        public string Keterangan { get; set; }
    }
}