# DOM Ağacı Görselleştirici Projesi (DOM Tree Visualizer)

Bu proje, özel bir C# arka ucu (backend) kullanarak HTML kodlarını ayrıştıран и elde edilen yapıyı ön yüzde (frontend) bir N-ary ağacı olarak görselleştiren web tabanlı bir DOM Ağacı Görselleştiricidir.

## Proje Yapısı
- **backend/DomParserApi**: Özel veri yapıları (Yığın/Stack, Kuyruk/Queue, Karma Tablo/HashTable) ve Regex tabanlı bir HTML ayrıştırıcı içeren ASP.NET Core Web API projesi.
- **wwwroot**: DOM ağacını görselleştirmek için geliştirilmiş modern kullanıcı arayüzü (UI).

## Nasıl Çalıştırılır?

### Seçenek 1: Docker (Önerilen)
Bilgisayarınızda Docker ve Docker Compose kuruluysa, projenin kök dizininde şu komutu çalıştırmanız yeterlidir:
```bash
docker-compose up --build
```

### Seçenek 2: Yerel .NET SDK
1. **Gereksinimler**: .NET SDK 8.0'ın kurulu olduğundan emin olun.
2. **Arka Uç Dizinine Gidin**:
   ```bash
   cd backend/DomParserApi
   ```
3. **Uygulamayı Çalıştırın**:
   ```bash
   dotnet run --launch-profile http
   ```

## Uygulamaya Erişin
Her iki yöntemle de uygulama başlatıldıktan sonra tarayıcınızdan şu adrese gidin:
[**http://localhost:5175**](http://localhost:5175)

## Özellikler
- **N-ary Ağaç Görselleştirme**: Ebeveyn-çocuk (parent-child) ilişkilerini doğru bir şekilde görüntüler.
- **Metin Düğümü (Text Node) Ayrıştırma**: Etiketlerin arasında kalan metin içeriklerini yakalar и başarıyla listeler.
- **Öznitelik (Attribute) Desteği**: Regex kullanarak birden fazla sınıfı (class) ve kimliği (ID) hatasız bir şekilde işler.
- **Arama Fonksiyonu**: Etiketler, sınıflar ve kimlikler için BFS (Genişlik Öncelikli Arama) tabanlı arama desteği sunar.
- **Docker Desteği**: Proje tamamen konteynerize edilmiştir (Dockerfile и docker-compose.yml).
