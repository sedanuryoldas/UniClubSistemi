# UniClub - Kampüs Etkinlik Yönetim Platformu

UniClub, üniversite kampüslerindeki öğrenci kulüplerini ve düzenledikleri etkinlikleri tek bir çatı altında toplayan web tabanlı bir etkinlik yönetim platformudur.

---

## 1. Proje Konusu ve Amacı

* **Proje Konusu:** Kampüs ekosistemindeki dağınıklığı gidererek kulüp faaliyetlerini dijitalleştiren yenilikçi bir web platformu.
* **Proje Amacı:** Kampüs içindeki etkinlik duyurularının WhatsApp grupları, panolardaki afişler ve dağınık sosyal medya hesapları arasında kaybolması problemini çözmek. Bu sistem sayesinde öğrenciler kendi ilgi alanlarına (müzik, yazılım, tiyatro vb.) uygun etkinlikleri ve kulüpleri kolayca keşfedebilirken; kulüp yöneticileri de etkinliklerini tüm kampüse çok daha hızlı, organize ve etkili bir şekilde duyurabilmektedir.

---

## 2. Hedef Kullanıcı Profilleri

Sistemimiz rol bazlı erişim kontrolü (RBAC) ile üç farklı kullanıcı profiline hitap etmektedir:

* **Öğrenciler (Standart Kullanıcılar):** Kampüsteki etkinlikleri keşfetmek, detaylarını incelemek ve beğendikleri etkinlikleri favorilerine ekleyerek kendi kişisel takvimlerini oluşturmak isteyen üniversite öğrencileri.
* **Kulüp Başkanları / Yöneticileri:** Kendilerine atanan yetkiler dahilinde, kendi kulüplerinin etkinliklerini sisteme ekleyen, güncelleyen ve duyuran yetkili öğrenciler.
* **Sistem Yöneticileri (Admin):** Platform düzenini sağlamak amacıyla sisteme yeni kulüpler ekleme, kural dışı içerikleri/etkinlikleri silme ve genel platform yönetimini gerçekleştirme yetkisine sahip yöneticiler.

---

## 3. Temel Özellikler

* **Dinamik Arama Motoru:** Kullanıcıların etkinlik adı, kulüp adı veya etkinlik detayına göre kelime bazlı filtreleme yapabildiği anlık arama altyapısı.
* **Kullanıcı Yetkilendirme (Auth):** Misafir kullanıcıların sadece etkinlikleri görüntüleyebildiği, kayıtlı kullanıcıların etkileşime geçebildiği ve Admin'lerin yönetim paneline erişebildiği gelişmiş rol yönetimi.
* **Etkinlik ve Kulüp Yönetimi (CRUD):** Admin ve yetkili kulüp hesaplarının; tarih, saat ve konum bilgisi girerek yeni etkinlik oluşturabilmesi, güncellemesi veya silmesi.
* **Responsive ve Modern UI:** Hem mobil cihazlarda hem de masaüstü bilgisayarlarda kusursuz çalışan, Bootstrap 5 tabanlı, kullanıcı dostu arayüz tasarımı.

---

## 4. Kullanılan Teknolojiler

| Katman | Teknoloji / Kütüphane | Kullanım Amacı |
| :--- | :--- | :--- |
| **Backend** | C# & ASP.NET Core MVC | Sunucu taraflı iş mantığı, yönlendirmeler ve Controller mimarisi. |
| **Veritabanı** | Firebase Firestore | Hızlı, ölçeklenebilir ve gerçek zamanlı NoSQL bulut veritabanı. |
| **Frontend** | HTML5, CSS3, JavaScript | Dinamik kullanıcı arayüzü ve istemci taraflı kontroller. |
| **CSS Framework**| Bootstrap 5 | Grid sistemi, modern kart yapıları, modal pencereler ve responsive tasarım. |
| **Versiyon Kontrol**| Git & GitHub | Ekip içi kod senkronizasyonu ve versiyon takibi. |

---

## 5. Veritabanı Yapısı (Firebase Firestore)

Projemizde ilişkisel olmayan (NoSQL) veritabanı mimarisi tercih edilmiştir. Veriler Koleksiyonlar (Collections) ve Dokümanlar (Documents) hiyerarşisinde tutulmaktadır:

### Etkinlikler Koleksiyonu (`Etkinlikler`)
Kampüste düzenlenen aktif ve geçmiş tüm etkinliklerin verilerini barındırır.
* `Id` (String) - Benzersiz etkinlik kimliği
* `Ad` (String) - Etkinlik başlığı
* `Aciklama` (String) - Etkinlik detay içeriği
* `Yer` (String) - Etkinliğin gerçekleşeceği konum/salon
* `Tarih` (Timestamp) - Etkinlik zamanı
* `KulupAdi` (String) - Etkinliği düzenleyen kulübün ismi

### Kulüpler Koleksiyonu (`Kulupler`)
Sisteme kayıtlı resmi öğrenci topluluklarını listeler.
* `Id` (String) - Benzersiz kulüp kimliği
* `Ad` (String) - Kulübün resmi adı
* `Aciklama` (String) - Kulübün misyonu ve kısa tanımı

### Favoriler Koleksiyonu (`Favoriler`)
Kullanıcılar ve etkinlikler arasındaki bağımsız ilişkiyi yönetir.
* `KullaniciEmail` (String) - Favori ekleyen kullanıcının e-posta adresi
* `EtkinlikId` (String) - Favoriye eklenen etkinliğin kimliği

> **Not:** Bu NoSQL yapı sayesinde, bir etkinlik silindiğinde veya kullanıcı verisi güncellendiğinde ilişkili kayıtlar Firestore üzerinde dinamik olarak eşleşerek hatasız bir şekilde getirilir.

---

## 6. Özgün Özellikler

UniClub platformunu standart bir kayıt/listeleme sisteminden ayıran iki temel akıllı motor bulunmaktadır:

1. **Dinamik Zaman Kontrolü (Akıllı Geçmiş Etkinlik Yönetimi):** Sistem, etkinliklerin veritabanındaki tarihi ile anlık zamanı (`DateTime.Now`) arka planda sürekli karşılaştırır. Tarihi geçmiş olan etkinlikler sistemden silinmez; ancak kullanıcı arayüzünde otomatik olarak soluklaştırırılarak "Geçmiş Etkinlik" etiketi alır. Ayrıca bu etkinlikler için "Favoriye Al" butonu otomatik olarak devre dışı bırakılır. Bu sayede manuel bir arşive gerek kalmadan takvim kendini güncel tutar.
2. **Kişiselleştirilmiş Favori Motoru:** Kullanıcıların etkinlikleri kaydetmesini sağlayan bu sistem, anlık olarak aktif kullanıcının e-posta adresi ile ilgili etkinliğin ID'sini Firestore üzerinde sorgular. Eğer etkinlik zaten favorilerdeyse buton dinamik olarak "Favoriden Çıkar" şekline dönüşür. Bu sayede her öğrenci kendine ait benzersiz bir kampüs etkinlik takvimi oluşturabilir.

---

## 7. Yapay Zeka (AI) Kullanım Beyanı

Bu projede yapay zeka (LLM tabanlı asistanlar), projeyi doğrudan yazdıran bir araç olarak değil; kodlama sürecini hızlandıran bir "pair-programmer" (eşli programcı) ve hata ayıklama (debugging) asistanı olarak konumlandırılmıştır.

* **Doğrudan Alınan Çıktılar:** Arayüz tasarımındaki bazı spesifik CSS gölgelendirme (`box-shadow`) ayarları, gradient arka plan renk kodları ve Bootstrap 5 grid sisteminin HTML iskeletleri (kart yapıları, modal pencereler) yapay zekadan optimize edilerek alınmıştır. Ayrıca gözden kaçan noktalama işareti ve yazım hataları (örneğin C#'taki ternary operatör kullanımındaki `CS1003` hataları) yapay zeka yardımıyla hızlıca debug edilmiştir.
* **Grup Tarafından Geliştirilen Özgün Bölümler:** Projenin genel yazılım mimarisi, NoSQL şemasına uygun veritabanı koleksiyon yapısının tasarlanması, Firebase'in native olarak ASP.NET Core MVC sistemine entegrasyonu, Controller (`HomeController`) içindeki veri çekme/gönderme mantığı, dinamik arama filtrelemesi ve yetkilendirme (Admin/User rolleri) algoritmaları tamamen grubumuz tarafından tasarlanmış ve kodlanmıştır.

> **Taahhüt:** Yapay zeka tarafından üretilen hiçbir kod öbeği, mantığı tamamen anlaşılmadan ve projenin mimarisine uygun hale getirilmeden sisteme dahil edilmemiştir. Sunum sırasında arka plandaki tüm C# ve veritabanı süreçleri tarafımızca savunulabilecek derinliktedir.

---

## 8. Grup Üyelerinin Katkıları ve Görev Dağılımı

Projemiz, ekip içerisindeki her bir üyenin kendi uzmanlık alanına göre teknik liderlik, veritabanı yönetimi ve kalite kontrol sorumluluklarını üstlenmesiyle başarılı bir şekilde tamamlanmıştır.

### Seda Nur YOLDAŞ — Team Lead & Frontend Developer
* **Proje Yönetimi ve Versiyon Kontrol:** GitHub reposunun sıfırdan kurulumunu, ana proje mimarisinin (MVC) oluşturulmasını ve tüm kodların ana dal (`main/master`) üzerinde çatışmasız bir şekilde yönetilmesini sağladı.
* **Arayüz Tasarımı (UI/UX):** Platformun mobil ve masaüstü cihazlarla tam uyumlu (responsive) çalışmasını sağlayan Bootstrap 5 entegrasyonunu kodladı. Etkinlik kartları, dinamik navigasyon menüsü ve arama barı bileşenlerini tasarladı.
* **Frontend Entegrasyonu:** Backend katmanından ve Firestore'dan gelen verilerin kullanıcıya dinamik olarak yansıtıldığı Razor View (`.cshtml`) sayfalarının geliştirilmesini ve veri bağlama (`data-binding`) süreçlerini yönetti.

### Mariam ABDELMAGID — Database & Firebase Specialist
* **Veritabanı Mimarisi:** Firebase Firestore bağlantılarını yapılandırarak projenin NoSQL veri modellemesini (Etkinlikler, Kulüpler ve Favoriler koleksiyonları arasındaki ilişkileri) tasarladı.
* **Bulut Entegrasyonu:** ASP.NET Core projesinin Google Firebase platformu ile olan asenkron veri alışverişini (`async/await` süreçlerini) yönetti ve temel veritabanı güvenlik kurallarını (`Security Rules`) kurguladı.
* **Veri Yönetimi:** Canlı demo ve sunum aşamasında kullanılacak olan gerçekçi test verilerinin sisteme girişini, koleksiyon bazlı veri optimizasyonlarını direkt olarak Firebase Konsolu üzerinden gerçekleştirdi.

### Amine ERDOĞAN — Backend Support & Quality Assurance (QA)
* **Mantıksal Algoritma Desteği:** Projenin en önemli özgün yanlarından biri olan, etkinliklerin tarihlerine göre anlık olarak "Geçmiş/Gelecek Etkinlik" şeklinde ayrılmasını sağlayan backend tabanlı C# algoritmalarının geliştirilmesine katkı sağladı.
* **Hata Takibi ve Test (QA):** Projenin teslimi öncesinde farklı kullanıcı senaryolarını simüle ederek dinamik arama filtresinin, rol bazlı yetkilendirmenin ve favori motorunun kararlılık testlerini yaptı; kod bloklarındaki mantıksal hataları (bug) tespit etti.
* **Raporlama ve Dokümantasyon:** Projenin teknik raporunu, bireysel katkı beyanlarını ve sunum materyallerini hazırlayarak projenin akademik teslim ve sunum standartlarına uygun hale getirilmesini sağladı.
