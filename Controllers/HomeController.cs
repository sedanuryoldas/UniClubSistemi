using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UniClubSistemi.Models;

namespace UniClubSistemi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // 1. Ana Sayfa: Herkesin görebildiði ilk karþýlama ekraný
        public IActionResult Index()
        {
            return View();
        }

        // 2. Etkinlik Takvimi: Paylaþýlan tüm etkinliklerin listelendiði sayfa
        public IActionResult Etkinlikler()
        {
            return View();
        }

        // 3. Kulüpler Rehberi: Üniversite bünyesindeki kulüplerin tanýtýmý
        public IActionResult Kulüpler()
        {
            return View();
        }

        // 4. Yetkilendirme - Giriþ: Kulüp baþkanlarýnýn yönetim paneline giriþi
        public IActionResult Giriþ()
        {
            return View();
        }

        // 5. Yetkilendirme - Kayýt: Yeni kulüplerin sisteme dahil olma talebi
        public IActionResult KayýtOl()
        {
            return View();
        }

        // 6. Etkinlik Yönetimi: Sadece giriþ yapmýþ yetkililerin göreceði form
        // (Þu an tasarým aþamasýnda olduðu için kapýsý herkese açýk)
        public IActionResult EtkinlikEkle()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}