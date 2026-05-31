# HTML to DOM Tree Parser

Bu proje, ham HTML metnini ayrıştırarak bellekte hiyerarşik bir DOM (Document Object Model) ağacına dönüştüren bir sistemin altyapısını oluşturur. Proje, web tarayıcılarının çalışma prensiplerini veri yapıları perspektifinden incelemeyi ve C# (.NET Core) üzerinde düşük seviyeli bellek/referans yönetimini simüle etmeyi amaçlamaktadır.

## 🚀 Proje Kapsamı ve Kurallar

Bu projede `System.Collections.Generic` altında bulunan `List<T>`, `Stack<T>`, `Queue<T>` veya `Dictionary<K, V>` gibi hazır standart kütüphaneler **kesinlikle kullanılmamıştır**. Tüm temel veri yapıları ve algoritmalar sıfırdan (from scratch) implemente edilmiştir.

---

## 🧩 Faz 1: Zorunlu Veri Yapıları (Data Structures)

Sistemin bellek yönetimi aşağıdaki özel veri yapıları ile sağlanmaktadır:

*   **N-ary Tree (`HtmlNode.cs`):** DOM ağacının temel yapı taşıdır. Hazır liste yerine **dinamik boyutlu dizi (dynamic array)** kullanılarak tasarlanmıştır. Kapasite dolduğunda kendini iki katına çıkarır (Amortized O(1) ekleme maliyeti). Her düğüm kendi ebeveynini ve çocuklarını referans olarak tutar.
*   **Yığıt / Stack (`CustomStack.cs`):** HTML ayrıştırma sırasında ağaç derinliğini takip etmek için **Tek Yönlü Bağlı Liste (Singly Linked List)** mimarisiyle yazılmıştır. `Push`, `Pop` ve `Peek` (en üstteki elemanı silmeden okuma) işlemleri O(1) zaman karmaşıklığında çalışır.
*   **Kuyruk / Queue (`CustomQueue.cs`):** Ağaç üzerinde BFS (Genişlik Öncelikli Arama) için tasarlanmıştır. O(1) ekleme/çıkarma performansı için **İki Uçlu Bağlı Liste (Head/Tail Pointer)** kullanılmıştır.
*   **Karma Tablo / Hash Table (`CustomHashTable.cs`):** `getElementById` sorgularını O(1) maliyetle çalıştırmak için tasarlanmıştır. Çakışma durumları (Collision) **Separate Chaining (Bağımsız Zincirleme)** yöntemi ile çözülmüştür.

---

## ⚙️ Faz 2: HTML Ayrıştırma ve DOM Ağacı Oluşturma (Parser)

`HtmlParser.cs` sınıfı, ham metin halindeki HTML'i okuyup N-ary Tree hiyerarşisini inşa eder.

### 1. Etiket Ayrıştırma (Tokenizer)
Düzenli ifadeler (Regex) yerine temel string manipülasyonu kullanılmıştır:
*   Algoritma `<` ve `>` karakterleri arasındaki metni çıkarır (Örn: `div id="main"`).
*   Çıkarılan metin bölünerek düğüm adı (`Tag`) ve özellikleri (`id`, `class`) dinamik olarak `HtmlNode` nesnelerine aktarılır.

### 2. Stack Tabanlı Hiyerarşi Yönetimi
Ağacın ebeveyn-çocuk ilişkisi, Yığıt (Stack) veri yapısının LIFO (Son Giren İlk Çıkar) prensibiyle kurgulanmıştır:
*   **Açılış Etiketi (<tag>):** Yeni bir düğüm (node) oluşturulur. Stack'in en tepesindeki elemana (`Peek()`) çocuk olarak eklenir ve kendisi Stack'e dahil edilir (`Push()`).
*   **Kapanış Etiketi (</tag>):** İlgili açılış etiketinin bloğunun bittiğini gösterir. Stack'in en üstündeki eleman çıkarılır (`Pop()`) ve ağaçta bir üst seviyeye dönülür.
*   **Self-Closing Etiketler:** `<img />` gibi kapanış gerektirmeyen etiketler algılanır, ağaca çocuk olarak eklenir ancak yığıta dahil edilmez.

### 3. Zaman ve Uzay Karmaşıklığı (Big-O Analizi)
*   **Zaman Karmaşıklığı: O(N)** - Ayrıştırıcı (Parser), HTML metni üzerinde baştan sona sadece bir kez gezinir (N = Karakter sayısı). Stack işlemleri O(1) olduğu için performans kaybı yaşanmaz.
*   **Uzay Karmaşıklığı: O(M)** - M, üretilen HTML etiketlerinin (düğümlerin) sayısıdır. Her etiket bellekte bir `HtmlNode` nesnesi olarak tutulur.

---

## 🛠️ Teknik Altyapı ve Katkı Sağlama (Git Akışı)

*   **Dil:** C# (.NET Core)
*   **Mimari:** OOP (Nesne Yönelimli Programlama) ve Kendi Kendini Yöneten Veri Yapıları.
*   **Versiyon Kontrolü:** `main` veya `master` dalına doğrudan kod atılmaz (Push engeli vardır). Yeni özellikler `feature/...` şeklinde dallar (branch) açılarak geliştirilir ve **Pull Request (PR)** mekanizması ile ana dala birleştirilir.
*   **İsimlendirme Şartı:** Takım üyelerinin isimleri, veritabanı kayıtları veya kod içindeki modüller Türkçe karakter içermeyen formatta yazılmıştır.