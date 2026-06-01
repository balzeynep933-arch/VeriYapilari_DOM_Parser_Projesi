using DomParserAPI.Models;

namespace DomParserAPI.Logic;

/// <summary>
/// Proje isterleri doğrultusunda tamamen sıfırdan (from scratch) yazılmış, 
/// Chaining (Bağlı Liste) yöntemiyle çakışmaları (collision) çözen Custom Hash Table yapısı.
/// </summary>
public class HashEntry
{
    public string Key { get; set; }
    public DomNode Value { get; set; }
    public HashEntry? Next { get; set; }

    public HashEntry(string key, DomNode value)
    {
        Key = key;
        Value = value;
    }
}

public class CustomHashTable
{
    private HashEntry?[] _buckets;
    private int _size;
    private const double LoadFactorThreshold = 0.75; // Tablo %75 doluluğa ulaşınca genişleyecek

    public CustomHashTable(int initialCapacity = 16)
    {
        _buckets = new HashEntry?[initialCapacity];
        _size = 0;
    }

    /// <summary>
    /// Horner Metodu (Polynomial Rolling Hash) kullanan etkili hash fonksiyonu.
    /// </summary>
    private int GetHash(string key)
    {
        uint hash = 0;
        foreach (char c in key)
        {
            hash = (hash * 31) + c;
        }
        return (int)(hash % _buckets.Length);
    }

    /// <summary>
    /// Verilen anahtar ve düğüm referansını O(1) ortalama karmaşıklıkla tabloya ekler.
    /// </summary>
    public void Put(string key, DomNode value)
    {
        if ((double)_size / _buckets.Length >= LoadFactorThreshold)
        {
            Resize();
        }

        int index = GetHash(key);
        HashEntry? current = _buckets[index];

        // Eğer aynı key varsa güncelle (id tektir ancak esneklik için)
        while (current != null)
        {
            if (current.Key == key)
            {
                current.Value = value;
                return;
            }
            current = current.Next;
        }

        // Chaining: Yeni elemanı bağlı listenin başına ekle
        var newEntry = new HashEntry(key, value) { Next = _buckets[index] };
        _buckets[index] = newEntry;
        _size++;
    }

    /// <summary>
    /// Anahtara göre O(1) karmaşıklıkta düğüm referansını getirir.
    /// </summary>
    public DomNode? Get(string key)
    {
        int index = GetHash(key);
        HashEntry? current = _buckets[index];

        while (current != null)
        {
            if (current.Key == key)
            {
                return current.Value;
            }
            current = current.Next;
        }

        return null; // Bulunamadı
    }

    /// <summary>
    /// Tablo dolduğunda performansı O(1)'de korumak için kapasiteyi 2 katına çıkarır ve rehash yapar.
    /// </summary>
    private void Resize()
    {
        int newCapacity = _buckets.Length * 2;
        HashEntry?[] oldBuckets = _buckets;
        _buckets = new HashEntry?[newCapacity];
        _size = 0;

        foreach (var entry in oldBuckets)
        {
            HashEntry? current = entry;
            while (current != null)
            {
                Put(current.Key, current.Value);
                current = current.Next;
            }
        }
    }
}
