namespace UniClubSistemi.Models
{
    public class Kulup
    {
        public string? Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Baskan { get; set; } = string.Empty;
        public string BaskanEmail { get; set; } = string.Empty; 
        public string Aciklama { get; set; } = string.Empty;
        public int UyeSayisi { get; set; } = 0;
        public string KurulusYili { get; set; } = string.Empty;
    }
}