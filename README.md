# veri-yap-lar-projesi
032390143_Front
DOM Ağacı Görselleştirici (DOM Tree Visualizer)
Bu proje, HTML kaynak kodunu analiz eden ve bu kodu etkileşimli, hiyerarşik bir ağaç yapısına (N-ary Tree) dönüştüren modern bir web aracıdır. "Cyberpunk" temalı karanlık bir arayüze ve yüksek performanslı algoritmalara sahiptir.
Öne Çıkan Özellikler
1)Anlık Analiz: HTML metnini milisaniyeler içinde işler ve görsel bir ağaç oluşturur.
2)Gelişmiş Arama:
-Etiket (tag), sınıf (class) ve ID bazlı arama yapabilir.
-ID aramalarında Hash Table yapısını kullanarak O(1) karmaşıklığında hızlı erişim sağlar.
-Bulunan sonuçların ebeveyn düğümlerini otomatik olarak genişletir.
3)Etkileşimli Arayüz:
-Düğümleri tek tek veya toplu olarak daraltıp genişletme özelliği.
-Derinlik göstergeleri (Depth Indicators) ile görsel hiyerarşi takibi.
-Dinamik animasyonlar ve derinlik bazlı renklendirme.
4)Kod Düzenleyici: Tab desteği ve Ctrl + Enter kısa yolu ile hızlı analiz imkanı.
Teknik Altyapı ve Algoritmalar
Proje, veri yapıları ve algoritmaların web üzerindeki pratik bir uygulamasıdır:
1)Tokenizasyon (Lexical Analysis): HTML dizisini etiketler ve metinler olarak parçalara ayırır. Kendi kendini kapatan etiketleri (void elements) tanır.
2)Yığın Tabanlı Ayrıştırma (Stack-based Parsing): Ağaç yapısını oluşturmak için Stack (Yığın) veri yapısı kullanılır. Bu sayede iç içe geçmiş (nested) etiketler hatasız bir şekilde hiyerarşiye aktarılır.
3)Hash Map İndeksleme: ID'ye göre yapılan aramaları optimize etmek için bir indeks tablosu tutulur.
4)Ağaç Gezinimi (Tree Traversal): Görselleştirme ve arama işlemleri için N-ary ağaç üzerinde BFS (Genişlik Öncelikli Arama) ve rekürsif yöntemler kullanılır.
Tasarım
1)Tema: Catppuccin Mocha tabanlı, göz yormayan karanlık mod.
2)Fontlar: Kodlar için JetBrains Mono, başlıklar için Syne.
3)Efektler: Retro terminal hissi veren tarama çizgileri (scanlines) ve yumuşak geçiş efektleri.
Kullanım Notları
1)Ctrl + Enter: HTML kodunu ayrıştırır.
2)Tab: Kod alanında girinti oluşturur.
3)Düğümlere tıklayarak alt dalları gizleyebilir veya açabilirsiniz.
