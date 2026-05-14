using Google.Cloud.Firestore;
using UniClubSistemi.Models;

var builder = WebApplication.CreateBuilder(args);

//  FİREBASE BAĞLANTISI 
// 1. Anahtar dosyamızın yolunu gösteriyoruz
string filepath = "firebase-key.json";
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);

// 2. Firebase Proje ID'mizi yazıyoruz
string projectId = "unietkinliksistemi";
builder.Services.AddSingleton<FirestoreDb>(FirestoreDb.Create(projectId));

// Servisleri konteynere ekliyoruz
builder.Services.AddControllersWithViews();
//  1. KİMLİK DOĞRULAMA (GİRİŞ YAPMA) AYARLARI 
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        // Giriş yapmamış biri yasaklı sayfaya girmeye çalışırsa onu buraya yönlendir:
        options.LoginPath = "/Home/Giriş";
    });

var app = builder.Build();

// HTTP istek işlem hattını yapılandırıyoruz
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//  2. KAPI GÜVENLİĞİNİ AKTİF ET 
app.UseAuthentication(); // Önce kimlik sor 
app.UseAuthorization();  // Sonra yetkiye bak 

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();