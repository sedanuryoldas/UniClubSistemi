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

        // Ana Sayfa
        public IActionResult Index()
        {
            return View();
        }

        // Etkinlikleri Listelediðimiz Sayfa
        public IActionResult Etkinlikler()
        {
            return View();
        }

        // Kulüplerin Listelendiði Sayfa
        public IActionResult Kulüpler()
        {
            return View();
        }

        // Yeni Etkinlik Ekleme Formunun Olduðu Sayfa
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