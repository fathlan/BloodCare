namespace BloodCare.ViewModels
{
    public class StokRingkasViewModel
    {
        public string Golongan { get; set; } = string.Empty;
        public int JumlahKantong { get; set; }
        public int Persen { get; set; }
    }

    public class LandingPageViewModel
    {
        public List<StokRingkasViewModel> StokPerGolongan { get; set; } = new();
        public int TotalPendonorAktif { get; set; }
        public int TotalKantongTersalurkan { get; set; }
        public int TotalJadwalAktif { get; set; }
    }
}
