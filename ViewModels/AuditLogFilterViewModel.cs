namespace BloodCare.ViewModels
{
    public class AuditLogFilterViewModel
    {
        public string? SearchUser { get; set; }
        public string? FilterAction { get; set; }
        public string? FilterTable { get; set; }
        public DateTime? TanggalDari { get; set; }
        public DateTime? TanggalSampai { get; set; }
    }
}
