# DOM Ağacı Görselleştirici

HTML kodunu ayrıştırarak tarayıcı benzeri bir DOM ağacı oluşturan ve bunu etkileşimli bir arayüzde görselleştiren web tabanlı bir araç. Tüm temel veri yapıları (yığıt, kuyruk, karma tablo) standart kütüphanelerden bağımsız olarak sıfırdan yazılmıştır.

## İçindekiler

- [Proje Hakkında](#proje-hakkında)
- [Özellikler](#özellikler)
- [Sistem Mimarisi](#sistem-mimarisi)
- [Veri Yapıları ve Algoritmalar](#veri-yapıları-ve-algoritmalar)
- [Kurulum ve Çalıştırma](#kurulum-ve-çalıştırma)
- [Takım ve Görev Dağılımı](#takım-ve-görev-dağılımı)

---

## Proje Hakkında

Bu proje, Veri Yapıları ve Algoritmalar dersi kapsamında geliştirilmiştir. Ham HTML metni; tokenizasyon, yığıt tabanlı ağaç inşası ve hash tablosu ile indeksleme aşamalarından geçirilerek görselleştirilebilir bir DOM ağacına dönüştürülmektedir.

**Arka uç:** ASP.NET Core 8.0 — RESTful API  
**Ön yüz:** Saf HTML / CSS / JavaScript — collapsible N-ary ağaç görünümü  
**İletişim:** HTTP/JSON, CORS tüm origin'lere açık

---

## Özellikler

- **N-ary Ağaç Görselleştirme** — ebeveyn-çocuk ilişkilerini doğru biçimde gösterir
- **Metin Düğümü Ayrıştırma** — etiketler arası metin içeriklerini yakalar
- **Öznitelik Desteği** — Regex ile birden fazla `class` ve `id` hatasız işlenir
- **Çift Modlu Arama** — `id` için O(1) hash tablosu, `tag/class` için BFS
- **Docker Desteği** — proje tamamen konteynerize edilmiştir

---

## Sistem Mimarisi

```
Kullanıcı (HTML girdisi)
        │
        ▼
POST /api/parse
        │
        ├─► HtmlTokenizer      →  Token listesi          O(N)
        ├─► DomTreeBuilder     →  N-ary Tree (Stack)     O(N)
        ├─► DomIndexer         →  Hash Table (DFS)       O(N)
        │
        ▼
  JSON yanıtı
        │
        ▼
  Frontend render (collapsible ağaç)
        │
        ▼
  Arama: id → Hash Map O(1)  |  tag/class → BFS O(N)
```

### Modül ve Dosya Yapısı

| Dosya | Sorumlu | Açıklama |
|---|---|---|
| `Logic/HtmlTokenizer.cs` | Mehmet Fatih Erduran | Lexical analysis, token üretimi, void element tespiti |
| `Models/Token.cs` | Mehmet Fatih Erduran | Token sınıfı ve TokenType enum |
| `Controllers/ParserController.cs` | Mehmet Fatih Erduran | REST endpoint, DI container entegrasyonu |
| `Logic/DomTreeBuilder.cs` | Kerem Bilgiç | Stack tabanlı N-ary Tree inşası |
| `Logic/CustomStack.cs` | Kerem Bilgiç | Generic Stack\<T\> (from scratch) |
| `Models/DomNode.cs` | Kerem Bilgiç | Ağaç düğümü — Children, Parent, Attributes |
| `DomParserApi/HtmlParser.cs` | Kerem Bilgiç | Alternatif parser — string manipülasyonu |
| `DomParserApi/HtmlNode.cs` | Kerem Bilgiç | Dinamik dizi tabanlı HtmlNode modeli |
| `Logic/CustomHashTable.cs` | Zeynep Bal | Chaining Hash Table, Horner hash fonksiyonu |
| `Logic/DomIndexer.cs` | Zeynep Bal | DFS ile id/class indeksleme servisi |
| `DomParserApi/CustomQueue.cs` | Nihad Gasimzade | Bağlı liste tabanlı Queue (from scratch) |
| `wwwroot/032390143_dom-front.html` | Rakhimzhan Abdrassulov | Cyberpunk temalı frontend, BFS arama |
| `DomParserApi/Dockerfile` | Rakhimzhan Abdrassulov | Multi-stage .NET 8 build imajı |
| `docker-compose.yml` | Rakhimzhan Abdrassulov | Servis tanımı, port 5175 yönlendirmesi |

---

## Veri Yapıları ve Algoritmalar

### Stack (Yığıt) — `CustomStack<T>`

HTML ayrıştırması sırasında açılış/kapanış etiketlerinin sırasını takip etmek için kullanılır. `List<T>` üzerine inşa edilmiş olup tüm temel işlemler O(1) karmaşıklığındadır. Hatalı HTML'de bile kısmi ağaç oluşturulabilmesini sağlar.

### Hash Table (Karma Tablo) — `CustomHashTable`

`getElementById` ve class bazlı aramaları O(1) ortalama karmaşıklıkta gerçekleştirmek için Separate Chaining yöntemiyle yazılmıştır.

- Başlangıç kapasitesi: 16 kova; yük faktörü %75'i geçince `Resize()` tetiklenir
- Hash fonksiyonu: Polynomial Rolling Hash — `hash = hash * 31 + c`
- `_idTable` ve `_classTable` olmak üzere iki bağımsız tablo tutulur

### Queue (Kuyruk) — `CustomQueue`

BFS algoritmasının doğrudan bağımlısıdır. Çift yönlü bağlı liste mantığıyla yazılmış olup `_head` ve `_tail` referansları O(1) ekleme/çıkarma sağlar. Bellek dinamik olarak büyür.

### Karmaşıklık Özeti

| İşlem | Ortalama | En Kötü | Uzay |
|---|---|---|---|
| HtmlTokenizer.Tokenize() | O(N) | O(N) | O(T) |
| Stack.Push / Pop / Peek | O(1) | O(1) | O(D) |
| DOM Ağacı İnşası | O(N) | O(N) | O(D) |
| HtmlNode.AddChild (amortize) | O(1) | O(N) resize | — |
| HashTable.Put / Get | O(1) | O(K) | O(N) |
| BFS / DFS Gezinme | O(N) | O(N) | O(W) / O(D) |

> N: token/düğüm sayısı · D: ağaç derinliği · W: ağaç genişliği · K: hash zinciri uzunluğu · T: token listesi boyutu

---

## Kurulum ve Çalıştırma

### Seçenek 1: Docker (Önerilen)

Docker ve Docker Compose kuruluysa proje kök dizininde şu komutu çalıştırın:

```bash
docker-compose up --build
```

### Seçenek 2: Yerel .NET SDK

1. .NET SDK 8.0 kurulu olduğundan emin olun.
2. Arka uç dizinine gidin:
   ```bash
   cd backend/DomParserApi
   ```
3. Uygulamayı başlatın:
   ```bash
   dotnet run --launch-profile http
   ```

### Uygulamaya Erişim

Her iki yöntemde de uygulama başladıktan sonra tarayıcıdan şu adrese gidin:

```
http://localhost:5175
```

---

## Takım ve Görev Dağılımı

| Ad Soyad | Öğrenci No | Üstlenilen Görev |
|---|---|---|
| **Mehmet Fatih Erduran** | 032490002 | `HtmlTokenizer` modülü — tek geçişli lexical analysis, token/attribute parsing (Regex), ASP.NET Core endpoint'leri ve CORS yapılandırması |
| **Kerem Bilgiç** | 032490001 | DOM ağacı inşası — `DomTreeBuilder`, `HtmlParser`, `CustomStack<T>`, `DomNode` ve `HtmlNode` modelleri (dinamik dizi genişletme dahil), Stack tabanlı ayrıştırma algoritması |
| **Zeynep Bal** | 032390103 | Hash Table implementasyonu — Polynomial Rolling Hash (Horner metodu), Separate Chaining ile çakışma çözümü, `DomIndexer` servisi (DFS ile id/class indeksleme), Load Factor yönetimi ve Resize mekanizması |
| **Nihad Gasimzade** | 032390146 | Ağaç gezinme algoritmaları — bağlı liste tabanlı `CustomQueue`, BFS (level-order traversal), DFS (rekürsif preorder), ağaç derinliği hesaplama ve kardeş düğüm bulma fonksiyonları |
| **Rakhimzhan Abdrassulov** | 032390143 | Frontend arayüzü — collapsible N-ary ağaç görselleştirmesi, çift modlu arama (hash + BFS), Cyberpunk/Catppuccin Mocha teması, JetBrains Mono/Syne fontları. Docker yapılandırması (Dockerfile, docker-compose.yml) |

