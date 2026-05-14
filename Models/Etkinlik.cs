using System;
using System.ComponentModel.DataAnnotations;

namespace UniClubSistemi.Models
{
    public class Etkinlik
    {
        // FİREBASE'İN OTOMATİK VERDİĞİ GİZLİ KİMLİĞİ TUTACAĞIMIZ ALAN
        public string? Id { get; set; }

        public string Ad { get; set; } = string.Empty;

        public string Aciklama { get; set; } = string.Empty;

        public DateTime Tarih { get; set; }

        public string Yer { get; set; } = string.Empty;

        public bool OnaylandiMi { get; set; }
    }
}