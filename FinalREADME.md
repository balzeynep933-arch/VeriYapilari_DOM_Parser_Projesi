DOM Ağacı Görselleştirici Projesi (DOM Tree Visualizer)
Bu proje, özel bir C# arka ucu (backend) kullanarak HTML kodlarını ayrıştıran ve elde edilen yapıyı ön yüzde (frontend) bir N-ary ağacı olarak görselleştiren web tabanlı bir DOM Ağacı Görselleştiricidir.

Proje Yapısı
backend/DomParserApi: Özel veri yapıları (Yığın/Stack, Kuyruk/Queue, Karma Tablo/HashTable) ve Regex tabanlı bir HTML ayrıştırıcı içeren ASP.NET Core Web API projesi.

wwwroot: DOM ağacını görselleştirmek için geliştirilmiş modern kullanıcı arayüzü (UI).

Nasıl Çalıştırılır?
Gereksinimler: Bilgisayarınızda .NET SDK'nın kurulu olduğundan emin olun.

Arka Uç Dizinine Gidin:
cd backend/DomParserApi

Uygulamayı Çalıştırın:
dotnet run --launch-profile http

Uygulamaya Erişin:
Tarayıcınızı açın ve şu adrese gidin: http://localhost:5175

Özellikler
N-ary Ağaç Görselleştirme: Ebeveyn-çocuk (parent-child) ilişkilerini doğru bir şekilde görüntüler.

Metin Düğümü (Text Node) Ayrıştırma: Etiketlerin arasında kalan metin içeriklerini yakalar ve başarıyla listeler.

Öznitelik (Attribute) Desteği: Regex kullanarak birden fazla sınıfı (class) ve kimliği (ID) hatasız bir şekilde işler.

Arama Fonksiyonu: Etiketler, sınıflar ve kimlikler için BFS (Genişlik Öncelikli Arama) tabanlı arama desteği sunar.
