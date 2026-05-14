using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UniClubSistemi.Models;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace UniClubSistemi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FirestoreDb _firestoreDb;

        public HomeController(ILogger<HomeController> logger, FirestoreDb firestoreDb)
        {
            _logger = logger;
            _firestoreDb = firestoreDb;
        }

        // 1. Ana Sayfa: Yaklaşan etkinlikleri çeker
        public async Task<IActionResult> Index()
        {
            List<Etkinlik> etkinlikListesi = new List<Etkinlik>();
            var snapshot = await _firestoreDb.Collection("Etkinlikler").GetSnapshotAsync();

            foreach (var doc in snapshot.Documents)
            {
                if (doc.Exists)
                {
                    var data = doc.ToDictionary();
                    Etkinlik e = new Etkinlik
                    {
                        Id = doc.Id,
                        Ad = data.ContainsKey("Ad") ? data["Ad"]?.ToString() ?? "İsimsiz" : "İsimsiz",
                        Aciklama = data.ContainsKey("Aciklama") ? data["Aciklama"]?.ToString() ?? "" : "",
                        Yer = data.ContainsKey("Yer") ? data["Yer"]?.ToString() ?? "" : ""
                    };

                    if (data.ContainsKey("Tarih"))
                    {
                        var ts = (Timestamp)data["Tarih"];
                        e.Tarih = ts.ToDateTime().ToLocalTime();
                    }
                    etkinlikListesi.Add(e);
                }
            }

            return View(etkinlikListesi.Where(e => e.Tarih >= DateTime.Now).OrderBy(e => e.Tarih).Take(3).ToList());
        }

        // 2. Etkinlik Takvimi
        public async Task<IActionResult> Etkinlikler(string aramaKelimesi)
        {
            List<Etkinlik> etkinlikListesi = new List<Etkinlik>();
            var snapshot = await _firestoreDb.Collection("Etkinlikler").GetSnapshotAsync();

            foreach (var doc in snapshot.Documents)
            {
                var data = doc.ToDictionary();

                string ad = data.ContainsKey("Ad") ? data["Ad"]?.ToString() ?? "İsimsiz" : "İsimsiz";
                string kulup = data.ContainsKey("DuzenleyenKulup") ? data["DuzenleyenKulup"]?.ToString() ?? "" : "";

                if (!string.IsNullOrEmpty(aramaKelimesi))
                {
                    // Hem etkinlik isminde hem de düzenleyen kulüp isminde arama yap (Büyük/küçük harf duyarsız)
                    bool adUyusuyor = ad.Contains(aramaKelimesi, StringComparison.OrdinalIgnoreCase);
                    bool kulupUyusuyor = kulup.Contains(aramaKelimesi, StringComparison.OrdinalIgnoreCase);

                    if (!adUyusuyor && !kulupUyusuyor)
                    {
                        continue;
                    }
                }

                Etkinlik e = new Etkinlik
                {
                    Id = doc.Id,
                    Ad = ad,
                    Aciklama = data.ContainsKey("Aciklama") ? data["Aciklama"]?.ToString() ?? "" : "",
                    Yer = data.ContainsKey("Yer") ? data["Yer"]?.ToString() ?? "" : ""
                };
                if (data.ContainsKey("Tarih")) e.Tarih = ((Timestamp)data["Tarih"]).ToDateTime().ToLocalTime();

                etkinlikListesi.Add(e);
            }

            List<string> favoriIds = new List<string>();
            if (User.Identity!.IsAuthenticated)
            {
                var favs = await _firestoreDb.Collection("Favoriler").WhereEqualTo("KullaniciEmail", User.Identity.Name).GetSnapshotAsync();
                favoriIds = favs.Documents.Select(d => d.GetValue<string>("EtkinlikId")).ToList();
            }

            ViewBag.Favorilenenler = favoriIds;

            // Arama kutusunda yazdığımız kelime sayfa yenilenince kaybolmasın diye View'e gönderiyoruz
            ViewBag.ArananKelime = aramaKelimesi;

            return View(etkinlikListesi.OrderBy(e => e.Tarih).ToList());
        }

        // 3. Kulüpler Rehberi
        public async Task<IActionResult> Kulüpler()
        {
            List<Kulup> kulupListesi = new List<Kulup>();
            var snapshot = await _firestoreDb.Collection("Kulupler").GetSnapshotAsync();

            foreach (var doc in snapshot.Documents)
            {
                var data = doc.ToDictionary();
                kulupListesi.Add(new Kulup
                {
                    Id = doc.Id,
                    Ad = data["Ad"]?.ToString() ?? "",
                    Baskan = data["Baskan"]?.ToString() ?? "",
                    BaskanEmail = data.ContainsKey("BaskanEmail") ? data["BaskanEmail"]?.ToString() ?? "" : "",
                    Aciklama = data["Aciklama"]?.ToString() ?? "",
                    UyeSayisi = data.ContainsKey("UyeSayisi") ? Convert.ToInt32(data["UyeSayisi"]) : 0,
                    KurulusYili = data["KurulusYili"]?.ToString() ?? ""
                });
            }
            return View(kulupListesi);
        }

        // 4. Kulüp Ekleme & Silme (Sadece Admin)
        [Authorize(Roles = "Admin")]
        public IActionResult KulupEkle() => View();

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> KulupEkle(Kulup yeniKulup)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                var veri = new Dictionary<string, object> {
                    { "Ad", yeniKulup.Ad },
                    { "Baskan", yeniKulup.Baskan },
                    { "BaskanEmail", yeniKulup.BaskanEmail },
                    { "Aciklama", yeniKulup.Aciklama },
                    { "UyeSayisi", yeniKulup.UyeSayisi },
                    { "KurulusYili", yeniKulup.KurulusYili }
                };
                await _firestoreDb.Collection("Kulupler").AddAsync(veri);
                return RedirectToAction("Kulüpler");
            }
            return View(yeniKulup);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> KulupSil(string id)
        {
            await _firestoreDb.Collection("Kulupler").Document(id).DeleteAsync();
            return RedirectToAction("Kulüpler");
        }

        // 5. Etkinlik Ekleme & Silme (Admin veya Kulüp Başkanı)
        [Authorize]
        public async Task<IActionResult> EtkinlikEkle()
        {
            string email = User.Identity!.Name!;
            bool isAdmin = User.IsInRole("Admin");
            List<string> yetkiliKulupler = new List<string>();

            var snapshot = await _firestoreDb.Collection("Kulupler").GetSnapshotAsync();
            foreach (var doc in snapshot.Documents)
            {
                var data = doc.ToDictionary();
                string baskanMail = data.ContainsKey("BaskanEmail") ? data["BaskanEmail"]?.ToString() ?? "" : "";

                if (isAdmin || baskanMail.Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    yetkiliKulupler.Add(data["Ad"]?.ToString() ?? "Bilinmeyen Kulüp");
                }
            }

            if (yetkiliKulupler.Count == 0 && !isAdmin) return RedirectToAction("Index");

            ViewBag.Kulupler = yetkiliKulupler;
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EtkinlikEkle(Etkinlik e, string KulupAdi)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                var veri = new Dictionary<string, object> {
                    { "Ad", e.Ad },
                    { "Aciklama", e.Aciklama },
                    { "Yer", e.Yer },
                    { "Tarih", Timestamp.FromDateTime(e.Tarih.ToUniversalTime()) },
                    { "DuzenleyenKulup", KulupAdi }
                };
                await _firestoreDb.Collection("Etkinlikler").AddAsync(veri);
                return RedirectToAction("Etkinlikler");
            }
            return RedirectToAction("EtkinlikEkle");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EtkinlikSil(string id)
        {
            await _firestoreDb.Collection("Etkinlikler").Document(id).DeleteAsync();
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }

        // 6. Giriş & Çıkış Sistemi
        public IActionResult Giriş() => View();

        [HttpPost]
        public async Task<IActionResult> Giriş(string? email, string? sifre)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sifre)) return View();

            var snapshot = await _firestoreDb.Collection("Kullanicilar")
                .WhereEqualTo("Email", email).WhereEqualTo("Sifre", sifre).GetSnapshotAsync();

            if (snapshot.Documents.Count > 0)
            {
                var userDoc = snapshot.Documents[0].ToDictionary();
                string rol = userDoc.ContainsKey("Rol") ? userDoc["Rol"]?.ToString() ?? "Kullanici" : "Kullanici";

                // Eğer admin değilse başkanlık kontrolü yap
                if (rol != "Admin")
                {
                    var baskanSnapshot = await _firestoreDb.Collection("Kulupler")
                        .WhereEqualTo("BaskanEmail", email).GetSnapshotAsync();
                    if (baskanSnapshot.Documents.Count > 0) rol = "Baskan";
                }

                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, email!),
                    new Claim(ClaimTypes.Role, rol)
                };
                await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies")));
                return RedirectToAction("Index");
            }
            ViewBag.Hata = "Hatalı giriş!";
            return View();
        }

        public async Task<IActionResult> CikisYap()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index");
        }

        public IActionResult KayıtOl() => View();

        [HttpPost]
        public async Task<IActionResult> KayıtOl(string email, string sifre)
        {
            var usersRef = _firestoreDb.Collection("Kullanicilar");
            var snap = await usersRef.WhereEqualTo("Email", email).GetSnapshotAsync();
            if (snap.Documents.Count > 0) return View();

            await usersRef.AddAsync(new Dictionary<string, object> { { "Email", email }, { "Sifre", sifre }, { "Rol", "Kullanici" } });
            return RedirectToAction("Giriş");
        }

        // 7. Favoriler Sistemi
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> FavoriToggle(string etkinlikId)
        {
            string email = User.Identity!.Name!;
            var favorilerRef = _firestoreDb.Collection("Favoriler");

            var snapshot = await favorilerRef.WhereEqualTo("KullaniciEmail", email).WhereEqualTo("EtkinlikId", etkinlikId).GetSnapshotAsync();

            if (snapshot.Documents.Count > 0)
            {
                foreach (var doc in snapshot.Documents) await doc.Reference.DeleteAsync();
            }
            else
            {
                await favorilerRef.AddAsync(new Dictionary<string, object> {
                    { "KullaniciEmail", email },
                    { "EtkinlikId", etkinlikId },
                    { "EklenmeTarihi", Timestamp.GetCurrentTimestamp() }
                });
            }
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }

        [Authorize]
        public async Task<IActionResult> Favorilerim()
        {
            string email = User.Identity!.Name!;
            var favSnapshot = await _firestoreDb.Collection("Favoriler").WhereEqualTo("KullaniciEmail", email).GetSnapshotAsync();
            var favIds = favSnapshot.Documents.Select(d => d.GetValue<string>("EtkinlikId")).ToList();

            List<Etkinlik> favoriListesi = new List<Etkinlik>();
            var etkinlikSnapshot = await _firestoreDb.Collection("Etkinlikler").GetSnapshotAsync();

            foreach (var doc in etkinlikSnapshot.Documents)
            {
                if (favIds.Contains(doc.Id))
                {
                    var data = doc.ToDictionary();
                    var e = new Etkinlik
                    {
                        Id = doc.Id,
                        Ad = data["Ad"]?.ToString() ?? "İsimsiz",
                        Aciklama = data["Aciklama"]?.ToString() ?? "",
                        Yer = data["Yer"]?.ToString() ?? ""
                    };
                    if (data.ContainsKey("Tarih")) e.Tarih = ((Timestamp)data["Tarih"]).ToDateTime().ToLocalTime();
                    favoriListesi.Add(e);
                }
            }

            ViewBag.Favorilenenler = favIds;
            return View(favoriListesi.OrderBy(e => e.Tarih).ToList());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}