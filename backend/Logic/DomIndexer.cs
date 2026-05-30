using DomParserAPI.Models;

namespace DomParserAPI.Logic;

/// <summary>
/// 3. Kişi Görevi: Oluşturulan DOM ağacını gezip id ve class niteliklerini 
/// sıfırdan yazdığımız Custom HashTable'a indeksleyen ve O(1) aramayı sağlayan servis.
/// </summary>
public class DomIndexer
{
    private readonly CustomHashTable _idTable = new();
    private readonly CustomHashTable _classTable = new(); // Opsiyonel class bazlı hızlı arama tablosu

    /// <summary>
    /// Ağacı DFS (Deep-First Search) mantığıyla rekürsif olarak gezip indeksleme yapar.
    /// </summary>
    public void IndexTree(DomNode root)
    {
        if (root == null) return;

        // 1. Düğümün 'id' niteliğini kontrol et ve indeksle
        if (root.Attributes.TryGetValue("id", out string? idValue) && !string.IsNullOrWhiteSpace(idValue))
        {
            _idTable.Put(idValue, root);
        }

        // 2. Düğümün 'class' niteliğini kontrol et ve indeksle (Opsiyonel ister)
        if (root.Attributes.TryGetValue("class", out string? classValue) && !string.IsNullOrWhiteSpace(classValue))
        {
            // Bir elemanın birden fazla class'ı olabilir (ör: class="btn btn-primary")
            string[] classes = classValue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var cls in classes)
            {
                _classTable.Put(cls, root);
            }
        }

        // 3. Alt düğümleri (Children) dolaşmaya devam et
        foreach (var child in root.Children)
        {
            IndexTree(child);
        }
    }

    /// <summary>
    /// Jüride hoca tarafından istenecek O(1) Zaman Karmaşıklığına sahip arama metodu.
    /// </summary>
    public DomNode? GetElementById(string id)
    {
        return _idTable.Get(id);
    }

    /// <summary>
    /// Class adına göre hızlı arama sağlayan metodu.
    /// </summary>
    public DomNode? GetElementByClassName(string className)
    {
        return _classTable.Get(className);
    }
}
