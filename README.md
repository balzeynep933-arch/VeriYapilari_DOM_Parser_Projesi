# DOM Ağacı Görselleştirici

Ham HTML metnini ayrıştırarak tarayıcı benzeri bir DOM ağacı oluşturan ve bunu etkileşimli arayüzde görselleştiren web tabanlı bir araç. Tüm temel veri yapıları — yığıt, kuyruk, karma tablo — standart kütüphanelerden bağımsız olarak **sıfırdan** yazılmıştır.

**Backend:** ASP.NET Core 8.0 — RESTful API  
**Frontend:** Saf HTML / CSS / JavaScript  
**Çalıştırma:** `docker-compose up --build` → `http://localhost:5175`

---

## İçindekiler

- [Sistem Mimarisi](#sistem-mimarisi)
- [Kurulum ve Çalıştırma](#kurulum-ve-çalıştırma)
- [Takım ve Modüller](#takım-ve-modüller)
  - [Mehmet Fatih Erduran — API & Parser Core](#1-mehmet-fatih-erduran--api--parser-core)
  - [Kerem Bilgiç — Stack & DOM Tree Builder](#2-kerem-bilgiç--stack--dom-tree-builder)
  - [Zeynep Bal — Hash Table](#3-zeynep-bal--hash-table)
  - [Nihad Gasimzade — Queue & Arama](#4-nihad-gasimzade--queue--arama)
  - [Rakhimzhan Abdrassulov — Frontend & Docker](#5-rakhimzhan-abdrassulov--frontend--docker)
- [Veri Yapıları ve Karmaşıklık Analizi](#veri-yapıları-ve-karmaşıklık-analizi)
- [API Referansı](#api-referansı)
- [Git İş Akışı](#git-iş-akışı)

---

## Sistem Mimarisi

```
Kullanıcı (HTML girdisi)
        │
        ▼
POST /api/parse              POST /api/parse/search
        │                            │
        ▼                            ▼
HtmlParser.ParseToTree()     HtmlParser.ParseToTree()
  └─ CustomStack                └─ CustomStack
  └─ HtmlNode (N-ary Tree)      └─ HtmlNode (N-ary Tree)
        │                            │
        ▼                            ▼
  JSON yanıtı               DomSearcher.BuildIndex()
        │                     └─ CustomQueue (BFS)
        ▼                     └─ CustomHashTable
  Frontend render                    │
  (collapsible ağaç)                 ▼
        │                   DomSearcher.Search()
        ▼                     ├─ id  → O(1) hash
  Arama sonucu vurgulama       └─ class/tag → O(N) BFS
```

### Proje Dizin Yapısı

```
VeriYapilari_DOM_Parser_Projesi/
├── 032390143_dom-front.html       # Frontend (Rakhimzhan)
├── docker-compose.yml             # Servis yapılandırması (Rakhimzhan)
├── README.md
└── backend/
    └── DomParserApi/
        ├── Controllers/
        │   └── ParseController.cs         # API endpoint'leri (Fatih)
        ├── Models/
        │   └── SearchRequest.cs           # İstek modeli (Fatih)
        ├── Parser/
        │   ├── HtmlParser.cs              # Ana parser algoritması (Kerem + Fatih)
        │   └── DomSearcher.cs             # Arama motoru (Nihad + Zeynep)
        ├── HtmlNode.cs                    # N-ary Tree düğümü (Kerem)
        ├── CustomStack.cs                 # Bağlı liste Stack (Kerem)
        ├── CustomHashTable.cs             # Separate Chaining Hash Table (Zeynep)
        ├── CustomQueue.cs                 # Bağlı liste Queue (Nihad)
        ├── Dockerfile                     # Multi-stage build (Rakhimzhan)
        └── Program.cs                     # Uygulama yapılandırması (Rakhimzhan)
```

---

## Kurulum ve Çalıştırma

### Seçenek 1: Docker (Önerilen)

Docker ve Docker Compose kurulu olduğundan emin olun, ardından proje kök dizininde:

```bash
docker-compose up --build
```

Tarayıcıdan erişim:

```
http://localhost:5175
```

### Seçenek 2: Yerel .NET SDK

1. [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) kurulu olduğundan emin olun.
2. Backend dizinine gidin:

```bash
cd backend/DomParserApi
```

3. Uygulamayı başlatın:

```bash
dotnet run --launch-profile http
```

4. Tarayıcıdan `http://localhost:5175` adresine gidin.

### Gereksinimlere Genel Bakış

| Gereksinim | Versiyon |
|---|---|
| .NET SDK | 8.0+ |
| Docker Engine | 24.0+ |
| Docker Compose | 2.0+ |

---

## Takım ve Modüller

---

### 1. Mehmet Fatih Erduran — API & Parser Core

**Öğrenci No:** 032490002  
**Branch:** `fatih/tokenizer`  
**Dosyalar:** `Controllers/ParseController.cs`, `Models/SearchRequest.cs`, `HtmlParser.cs` (ParseTagContent + ExtractAttribute metodları), `Program.cs` (CORS yapılandırması)

#### Sorumluluk

Backend'in dış dünyayla konuştuğu katman. İki REST endpoint'i ve bu endpoint'lere gelen isteği `HtmlParser`'a yönlendiren `ParseController`'ı yazdı. Ayrıca `HtmlParser` içindeki etiket içeriği ayrıştırma (tag name, id, class extraction) kısmından da sorumlu.

#### ParseController

```csharp
[ApiController]
[Route("api/[controller]")]
public class ParseController : ControllerBase
{
    // POST /api/parse
    // Body: JSON string (ham HTML metni)
    // Döndürür: kök HtmlNode'u JSON olarak
    [HttpPost]
    public ActionResult<HtmlNode> Post([FromBody] string html)

    // POST /api/parse/search
    // Body: { Html: "...", Query: "id=\"header\"" }
    // Döndürür: eşleşen HtmlNode listesini JSON olarak
    [HttpPost("search")]
    public ActionResult<List<HtmlNode>> Search([FromBody] SearchRequest request)
}
```

#### SearchRequest Modeli

```csharp
public class SearchRequest
{
    public string Html  { get; set; }   // Ayrıştırılacak HTML metni
    public string Query { get; set; }   // id="...", class="..." veya tag adı
}
```

#### ParseTagContent & ExtractAttribute

`HtmlParser.cs` içinde etiket içeriğinden tag adı ve öznitelikleri çıkaran metodlar Fatih tarafından yazıldı:

```csharp
// Regex ile tag adını çıkarır — tire içeren özel etiketleri (data-*) de destekler
private HtmlNode ParseTagContent(string content)
{
    var tagMatch = Regex.Match(content, @"^([a-zA-Z0-9\-]+)");
    string tagName = tagMatch.Success ? tagMatch.Groups[1].Value : "unknown";
    string id        = ExtractAttribute(content, "id");
    string className = ExtractAttribute(content, "class");
    return new HtmlNode(tagName, id, className) { Index = _nodeIndex++ };
}

// Hem tek hem çift tırnaklı attribute değerlerini yakalar
private string ExtractAttribute(string content, string attrName)
{
    var match = Regex.Match(content,
        attrName + @"\s*=\s*[""']([^""']*)[""']",
        RegexOptions.IgnoreCase);
    return match.Success ? match.Groups[1].Value : "";
}
```

**Regex kararı:** `[""'][^""']*[""']` pattern'i, hem `id="main"` hem de `id='main'` formatını yakalar. Boşluk içeren class değerlerinde (`class="btn primary"`) tüm değeri doğru çeker.

#### CORS Yapılandırması (Program.cs)

Geliştirme sürecinde frontend'in farklı port/origin'den API'ye erişebilmesi için tüm origin'lere izin verildi:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
```

`MapFallbackToFile("032390143_dom-front.html")` — Docker ortamında frontend HTML dosyası statik olarak serve ediliyor; bilinmeyen rotalara gelen istekler bu dosyaya yönlendiriliyor.

---

### 2. Kerem Bilgiç — Stack & DOM Tree Builder

**Öğrenci No:** 032490001  
**Branch:** `kerem/tree-builder`  
**Dosyalar:** `CustomStack.cs`, `HtmlNode.cs`, `HtmlParser.cs` (ParseToTree metodu)

#### Sorumluluk

Projenin kalbi. Ham HTML metnini alıp N-ary ağaca dönüştüren `ParseToTree()` algoritmasını ve bu algoritmanın bağımlı olduğu iki veri yapısını (`CustomStack`, `HtmlNode`) sıfırdan yazdı.

#### CustomStack — Bağlı Liste Tabanlı Yığıt

```
Bellekteki yapı (örnek: 3 açık etiket):

_top
  │
  ▼
[div] → [body] → [html] → null
```

```csharp
// Dahili bağlı liste düğümü
internal class StackNode
{
    public HtmlNode Data { get; set; }
    public StackNode Next { get; set; }
}

public class CustomStack
{
    private StackNode _top;   // her zaman yığıtın tepesini gösterir

    void Push(HtmlNode node)  // yeni StackNode oluştur, _top'a bağla   → O(1)
    HtmlNode Pop()            // _top.Data al, _top = _top.Next          → O(1)
    HtmlNode Peek()           // _top?.Data — çıkarmadan bak             → O(1)
    bool IsEmpty()            // _top == null                            → O(1)
}
```

**Neden bağlı liste?** Dizi tabanlı bir stack'te kapasite dolunca yeniden boyutlandırma (O(N)) gerekir. Bağlı liste, her Push'ta sabit maliyetle yeni bellek alır; ağaç derinliği değişken olduğundan bu daha uygun.

#### HtmlNode — N-ary Tree Düğümü

```
Bir HtmlNode örneği (bellekte):

┌──────────────────────────────────────┐
│ Tag:       "div"                     │
│ Id:        "container"               │
│ ClassName: "wrapper main"            │
│ Text:      null                      │
│ Index:     7  (frontend eşleştirme)  │
│ Parent:    → [body] (JsonIgnore)     │
│ _children: [HtmlNode*, HtmlNode*, …] │
│ ChildCount: 3                        │
└──────────────────────────────────────┘
```

Çocuk listesi **manuel dinamik dizi** ile yönetilir — standart `List<T>` kullanılmadı:

```csharp
public void AddChild(HtmlNode child)
{
    child.Parent = this;

    if (ChildCount == _children.Length)          // kapasite doldu mu?
    {
        HtmlNode[] newArray = new HtmlNode[_children.Length * 2];
        for (int i = 0; i < _children.Length; i++)
            newArray[i] = _children[i];          // manuel kopyalama
        _children = newArray;
    }
    _children[ChildCount] = child;
    ChildCount++;
}
```

- Başlangıç kapasitesi: **2**
- Dolunca: **2x büyüme** (amortize O(1))
- `[JsonIgnore]` on Parent: döngüsel JSON serializasyonunu engeller

#### HtmlParser.ParseToTree() — Stack Tabanlı Ağaç İnşası

Tek geçişli (single-pass) algoritma. Stack her zaman "şu an içinde bulunulan açık etiketler zinciri"ni temsil eder:

```
<html>               Stack: [document] → [html]
  <body>             Stack: [document] → [html] → [body]
    <div id="x">     Stack: [document] → [html] → [body] → [div#x]
      <p>text</p>    Stack: [document] → [html] → [body] → [div#x]  (p açılır kapanır)
    </div>           Stack: [document] → [html] → [body]   (div pop)
  </body>            Stack: [document] → [html]
</html>              Stack: [document]
```

Algoritmanın adımları:

```
index = 0
while index < html.Length:
    openBracket = html.IndexOf('<', index)
    
    eğer openBracket > index:
        → aradaki metin #text düğümü olarak Peek()'e ekle
    
    closeBracket = html.IndexOf('>', openBracket)
    tagContent = html[openBracket+1 .. closeBracket]
    
    eğer tagContent "!" veya "?" ile başlıyorsa:
        → yorum/DOCTYPE → atla
    
    eğer tagContent "/" ile başlıyorsa:
        → kapanış etiketi → stack.Pop()
    
    değilse:
        → ParseTagContent() ile HtmlNode oluştur
        → otomatik kapanış: li içinde li, td içinde td vs. → öncekini pop et
        → Peek()'e çocuk olarak ekle
        → void element değilse → stack.Push(newNode)
        → script/style ise → kapanış etiketine kadar içeriği tek text node yap
    
    index = closeBracket + 1
```

**Karmaşıklık:** Zaman O(N), Uzay O(D) — D: maksimum ağaç derinliği (Stack boyutu)

---

### 3. Zeynep Bal — Hash Table

**Öğrenci No:** 032390103  
**Branch:** `zeynep/hashtable`  
**Dosyalar:** `CustomHashTable.cs`, `DomSearcher.cs` (BuildIndex metodu)

#### Sorumluluk

`getElementById` işlemini ortalama O(1) karmaşıklıkta gerçekleştiren `CustomHashTable` veri yapısını ve ağacı BFS ile gezerek bu tabloyu dolduran `DomSearcher.BuildIndex()` metodunu yazdı.

#### CustomHashTable — Separate Chaining

```
Kapasite: 100 kova (bucket)

Bellek görünümü:

index 0:  null
index 1:  [header → <header#header>] → null
index 2:  null
index 3:  [nav → <nav#nav>] → [main → <div#main>] → null  ← çakışma (collision)
...
index 99: null
```

```csharp
public class CustomHashTable
{
    private readonly int _capacity = 100;
    private HashEntry[] _buckets;

    // Polynomial Rolling Hash — Horner metodu
    private int GetHash(string key)
    {
        int hash = 0;
        for (int i = 0; i < key.Length; i++)
            hash = (hash * 31 + key[i]) % _capacity;
        return Math.Abs(hash);
    }

    // O(1) ortalama — çakışmada zincirin sonuna ekle
    public void Insert(string key, HtmlNode node)

    // O(1) ortalama — hash ile kovayı bul, zincirde ara
    public HtmlNode Get(string key)
}
```

**Hash fonksiyonu seçimi:** `hash = hash * 31 + c` formülü (Horner metodu). 31'in seçimi tesadüf değil: asal sayı olması, bitwise işlemlerle verimli hesaplanması ve karakter dağılımında düşük çakışma oranı vermesi nedeniyle Java'nın `String.hashCode()` implementasyonunda da kullanılır.

**Separate Chaining:** Çakışma durumunda aynı kovaya birden fazla `HashEntry` eklenir; her `HashEntry` bir sonrakine `Next` pointer'ıyla bağlıdır. Yeni eleman zincirin sonuna eklenir; var olan anahtar bulunursa değeri güncellenir.

```csharp
internal class HashEntry
{
    public string   Key   { get; set; }
    public HtmlNode Value { get; set; }
    public HashEntry Next { get; set; }   // bağlı liste zinciri
}
```

**Not:** Bu implementasyonda Resize mekanizması yoktur; kapasite 100 kovada sabittir. Yüzlerce düğüm içeren orta ölçekli HTML dokümanlarında load factor yönetilebilir düzeyde kalır.

#### DomSearcher.BuildIndex() — BFS ile İndeks Oluşturma

Arama nesnesi oluşturulurken ağaç bir kez BFS ile geçilir, tüm id'ler hash tablosuna yazılır:

```csharp
private void BuildIndex()
{
    CustomQueue queue = new CustomQueue();
    queue.Enqueue(_root);

    while (!queue.IsEmpty())
    {
        HtmlNode current = queue.Dequeue();

        if (!string.IsNullOrEmpty(current.Id))
            _idTable.Insert(current.Id, current);   // O(1)

        foreach (var child in current.Children)
            if (child != null) queue.Enqueue(child);
    }
}
```

**Neden BFS?** DFS de aynı işi yapardı. Ancak BFS seviye sırasıyla çalıştığı için olası id çakışmalarında (aynı id iki kez geçiyorsa) üst seviyedeki düğüm tabloya önce girer ve alt düzey onu ezmez — bu daha öngörülebilir bir davranış.

---

### 4. Nihad Gasimzade — Queue & Arama

**Öğrenci No:** 032390146  
**Branch:** `nihad/tree-traversal`  
**Dosyalar:** `CustomQueue.cs`, `DomSearcher.cs` (Search metodu)

#### Sorumluluk

BFS algoritmasının omurgası olan `CustomQueue` veri yapısını ve farklı sorgu tiplerine göre akıllıca strateji seçen `DomSearcher.Search()` metodunu yazdı. Ağaç derinliği ve kardeş düğüm analiz fonksiyonlarından da sorumlu.

#### CustomQueue — Bağlı Liste Tabanlı Kuyruk

```
Bellekteki yapı (4 eleman):

_head                           _tail
  │                               │
  ▼                               ▼
[div] → [header] → [nav] → [main] → null

Enqueue: sağdan ekle (_tail'e bağla)
Dequeue: soldan al  (_head'i ilerlet)
```

```csharp
public class CustomQueue
{
    private QueueNode _head;   // çıkış ucu
    private QueueNode _tail;   // giriş ucu

    // O(1) — yeni QueueNode oluştur, _tail.Next = newNode, _tail = newNode
    public void Enqueue(HtmlNode node)

    // O(1) — _head.Data al, _head = _head.Next; ikisi de null olursa _tail = null
    public HtmlNode Dequeue()

    // O(1) — _head == null
    public bool IsEmpty()
}
```

**Neden iki pointer?** Sadece `_head` ile de kuyruk yapılabilir ama `Enqueue`'da zinciri sonuna kadar gezmek gerekir → O(N). `_tail` pointer'ı ekleyince hem `Enqueue` hem `Dequeue` O(1) olur.

#### DomSearcher.Search() — Hibrit Arama Stratejisi

```
Sorgu geldi
    │
    ├─ id="..." formatında mı?
    │       ↓ EVET
    │   _idTable.Get(id)          → O(1)  ✓
    │
    └─ class="..." veya düz metin?
            ↓
        BFS ile ağacı tara         → O(N)
        ├─ class="..." → ClassName.Contains(target)
        └─ düz metin  → Tag, Id, ClassName, Text içinde case-insensitive arama
```

```csharp
public List<HtmlNode> Search(string query)
{
    var idMatch    = Regex.Match(query, @"id\s*=\s*[""']?([\w-]+)[""']?");
    var classMatch = Regex.Match(query, @"class\s*=\s*[""']?([\w-]+)[""']?");

    // O(1) — Hash Table
    if (idMatch.Success)
    {
        HtmlNode node = _idTable.Get(idMatch.Groups[1].Value);
        if (node != null) results.Add(node);
        return results;
    }

    // O(N) — BFS
    CustomQueue queue = new CustomQueue();
    queue.Enqueue(_root);
    while (!queue.IsEmpty())
    {
        HtmlNode current = queue.Dequeue();
        bool hit = false;

        if (classMatch.Success)
            hit = current.ClassName?.Contains(classMatch.Groups[1].Value) ?? false;
        else
            hit = current.Tag?.Contains(query, OrdinalIgnoreCase) == true ||
                  current.Id?.Contains(query, OrdinalIgnoreCase)  == true ||
                  current.ClassName?.Contains(query, ...)          == true ||
                  current.Text?.Contains(query, ...)               == true;

        if (hit) results.Add(current);
        foreach (var child in current.Children)
            if (child != null) queue.Enqueue(child);
    }
    return results;
}
```

#### Ağaç Analiz Fonksiyonları

```csharp
// Ağaç derinliği — rekürsif
// Zaman: O(N), Uzay: O(D) call stack
int GetDepth(HtmlNode node)
{
    if (node.ChildCount == 0) return 0;
    int max = 0;
    foreach (var child in node.Children)
        max = Math.Max(max, GetDepth(child));
    return max + 1;
}

// Kardeş düğümler — parent üzerinden
// Zaman: O(K), K: parent'ın çocuk sayısı
IEnumerable<HtmlNode> GetSiblings(HtmlNode node)
    => node.Parent?.Children.Where(c => c != node) ?? Enumerable.Empty<HtmlNode>();
```

---

### 5. Rakhimzhan Abdrassulov — Frontend & Docker

**Öğrenci No:** 032390143  
**Branch:** `rakhim/frontend`  
**Dosyalar:** `032390143_dom-front.html`, `Dockerfile`, `docker-compose.yml`, `Program.cs`

#### Sorumluluk

Projenin görünen yüzü ve dağıtım altyapısı. 611 satır saf HTML/CSS/JavaScript ile yazılmış Cyberpunk temalı arayüzü ve projeyi tek komutla ayağa kaldıran Docker yapılandırmasını hazırladı.

#### Frontend Mimarisi

```
032390143_dom-front.html
├── <head>
│   ├── Google Fonts (JetBrains Mono, Syne)
│   └── <style> — CSS değişkenleri, layout, animasyonlar (650+ satır CSS)
│
├── <body>
│   ├── #topbar — logo, arama çubuğu, parse butonu
│   ├── #main
│   │   ├── #left  — #html-input (textarea, Tab desteği, Ctrl+Enter)
│   │   └── #right — #tree-scroll (collapsible ağaç render alanı)
│   └── #statusbar — düğüm sayısı, derinlik, durum mesajı
│
└── <script> — IIFE (async)
    ├── renderNode(node, depth)   — rekürsif DOM render
    ├── doSearch()                — backend'e /api/parse/search isteği
    ├── run()                     — backend'e /api/parse isteği
    └── event listeners
```

#### renderNode() — Rekürsif Ağaç Render

Backend'den gelen JSON ağacı, tarayıcı DevTools görünümünde HTML elementlere dönüştürülür:

```javascript
function renderNode(node, depth) {
    const nIndex = node.Index !== undefined ? node.Index : node.index;

    // #text düğümleri için özel görünüm
    if (nTag === '#text') {
        const d = document.createElement('div');
        d.className = 'text-leaf nr';
        d.dataset.index = nIndex;   // arama vurgulama için backend index eşleştirmesi
        // "metin içeriği" görünümü
        return d;
    }

    // Element düğümleri
    const wrap = document.createElement('div');   // .tn
    const row  = document.createElement('div');   // .nr
    row.dataset.index = nIndex;
    row.dataset.id    = nId;
    row.dataset.class = nClass;
    row.dataset.tag   = nTag;
    allRows.push(row);   // global liste — arama vurgulama için

    // <tagName id="..." class="...">
    // toggle (▾/▸) — click ile children div'ini göster/gizle
    
    if (hasKids) {
        const kids = document.createElement('div');
        kids.className = 'children';
        for (const ch of childrenArray)
            kids.appendChild(renderNode(ch, depth + 1));   // rekürsif
        
        row.addEventListener('click', () => {
            const collapsed = kids.style.display === 'none';
            kids.style.display = collapsed ? '' : 'none';
            // </tagName> satırı da göster/gizle
        });
    }
    return wrap;
}
```

**Index eşleştirmesi:** Backend her `HtmlNode`'a bir `Index` alanı atar. Search sonucu gelen düğümlerin index'i `allRows.find(r => r.dataset.index === nIndex)` ile frontend'deki elemana eşlenir ve `row.classList.add('hl')` ile vurgulanır.

#### doSearch() — Backend ile Arama Entegrasyonu

```javascript
async function doSearch() {
    const q    = document.getElementById('search-input').value.trim();
    const html = document.getElementById('html-input').value;

    // POST /api/parse/search → { html, query }
    const response = await fetch('/api/parse/search', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ html, query: q })
    });

    const results = await response.json();   // HtmlNode[]

    for (const node of results) {
        const row = allRows.find(r => parseInt(r.dataset.index) === node.Index);
        if (row) row.classList.add('hl');
    }

    // İlk eşleşmeye scroll + tüm atalarını aç
    if (firstHit) {
        let p = firstHit.parentElement;
        while (p && p.id !== 'tree-scroll') {
            if (p.classList.contains('children')) p.style.display = '';
            p = p.parentElement;
        }
        firstHit.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
}
```

#### Klavye Kısayolları

| Kısayol | İşlev |
|---|---|
| `Ctrl + Enter` | HTML'i ayrıştır (Parse) |
| `Tab` (textarea içinde) | 2 boşluk girintisi ekle |

#### Tema ve Tasarım

```css
:root {
    --bg:      #0a0c10;   /* ana arka plan */
    --accent:  #5ddcff;   /* cyan vurgu */
    --green:   #a6e3a1;   /* başarı/bulundu */
    --pink:    #f38ba8;   /* hata/bulunamadı */
    --tag:     #89dceb;   /* HTML tag rengi */
    /* Catppuccin Mocha paleti */
}
```

- **JetBrains Mono** — kod alanı ve ağaç görünümü
- **Syne** — başlıklar ve logo
- **Scanline efekti** — `body::before` ile repeating-linear-gradient
- **Pulse animasyonu** — logo'daki dot (2s ease-in-out)

#### Dockerfile — Multi-Stage Build

```dockerfile
# Aşama 1: Derleme (SDK imajı — ~800MB)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DomParserApi.csproj", "./"]
RUN dotnet restore "DomParserApi.csproj"
COPY . .
RUN dotnet publish "DomParserApi.csproj" -c Release -o /app/publish \
    /p:UseAppHost=false

# Aşama 2: Çalıştırma (Runtime imajı — ~200MB)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5175
ENV ASPNETCORE_URLS=http://+:5175
ENTRYPOINT ["dotnet", "DomParserApi.dll"]
```

**Multi-stage avantajı:** Son imaj SDK içermez, yalnızca runtime içerir. Bu sayede imaj boyutu ~800MB'dan ~200MB'a düşer.

#### docker-compose.yml

```yaml
services:
  dom-parser:
    container_name: dom_parser_app
    build:
      context: ./backend/DomParserApi
      dockerfile: Dockerfile
    ports:
      - "5175:5175"
    restart: always
```

---

## Veri Yapıları ve Karmaşıklık Analizi

| İşlem | Ortalama | En Kötü | Uzay |
|---|---|---|---|
| HtmlParser.ParseToTree() | O(N) | O(N) | O(D) — Stack |
| CustomStack.Push / Pop / Peek | O(1) | O(1) | O(D) |
| HtmlNode.AddChild — amortize | O(1) | O(N) resize | — |
| CustomHashTable.Insert | O(1) | O(K) | O(N) |
| CustomHashTable.Get | O(1) | O(K) | O(1) |
| CustomQueue.Enqueue / Dequeue | O(1) | O(1) | O(W) |
| DomSearcher.BuildIndex — BFS | O(N) | O(N) | O(W) |
| DomSearcher.Search — id | O(1) | O(K) | O(1) |
| DomSearcher.Search — class/tag | O(N) | O(N) | O(W) |
| Ağaç Derinliği Hesaplama | O(N) | O(N) | O(D) |
| Kardeş Düğüm Bulma | O(K) | O(K) | O(1) |

> **N:** düğüm/karakter sayısı · **D:** ağaç derinliği · **W:** ağaç genişliği · **K:** hash zinciri / çocuk sayısı

---

## API Referansı

### POST /api/parse

HTML metnini ayrıştırır, N-ary DOM ağacını döndürür.

**İstek:**
```http
POST /api/parse
Content-Type: application/json

"<html><body><h1 id=\"title\">Hello</h1></body></html>"
```

**Yanıt:**
```json
{
  "tag": "document",
  "id": "",
  "className": "",
  "text": null,
  "index": 0,
  "children": [
    {
      "tag": "html",
      "index": 1,
      "children": [...]
    }
  ]
}
```

### POST /api/parse/search

HTML'i ayrıştırır ve sorguyla eşleşen düğümleri döndürür.

**İstek:**
```http
POST /api/parse/search
Content-Type: application/json

{
  "html": "<div id=\"header\"><nav class=\"menu\">...</nav></div>",
  "query": "id=\"header\""
}
```

**Sorgu formatları:**

| Format | Örnek | Strateji |
|---|---|---|
| id araması | `id="header"` | O(1) — Hash Table |
| class araması | `class="menu"` | O(N) — BFS |
| tag araması | `nav` | O(N) — BFS |
| metin araması | `Hello` | O(N) — BFS |

**Yanıt:**
```json
[
  { "tag": "div", "id": "header", "index": 1, "children": [...] }
]
```

---

## Git İş Akışı

Main dalına doğrudan commit gönderilmedi. Her modül ayrı branch'te geliştirildi, Pull Request ile birleştirildi.

| Üye | Branch | Modül |
|---|---|---|
| Mehmet Fatih Erduran | `fatih/tokenizer` | ParseController, SearchRequest, ParseTagContent, ExtractAttribute |
| Kerem Bilgiç | `kerem/tree-builder` | CustomStack, HtmlNode, HtmlParser.ParseToTree() |
| Zeynep Bal | `zeynep/hashtable` | CustomHashTable, DomSearcher.BuildIndex() |
| Nihad Gasimzade | `nihad/tree-traversal` | CustomQueue, DomSearcher.Search(), ağaç analiz fonksiyonları |
| Rakhimzhan Abdrassulov | `rakhim/frontend` | Frontend, Dockerfile, docker-compose.yml |


