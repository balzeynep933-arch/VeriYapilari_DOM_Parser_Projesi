using System;

public class HashEntry
{
    public string Id;
    public DOMNode Node;
    public HashEntry Next;

    public HashEntry(string id, DOMNode node)
    {
        Id = id;
        Node = node;
        Next = null;
    }
}

public class HashTable
{
    private const int TABLE_SIZE = 1000;
    private HashEntry[] buckets;
    public int Count { get; private set; }

    public HashTable()
    {
        buckets = new HashEntry[TABLE_SIZE];
        Count = 0;
    }

    // Hash fonksiyonu
    private int HashFunc(string key)
    {
        ulong value = 0;

        foreach (char c in key)
        {
            value = value * 37 + c;
        }

        return (int)(value % TABLE_SIZE);
    }

    // Ekleme
    public void Insert(string id, DOMNode node)
    {
        if (string.IsNullOrEmpty(id)) return;

        int index = HashFunc(id);

        HashEntry newEntry = new HashEntry(id, node);

        // chaining (başa ekleme)
        newEntry.Next = buckets[index];
        buckets[index] = newEntry;

        Count++;
    }

    // Arama
    public DOMNode Lookup(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        int index = HashFunc(id);
        HashEntry current = buckets[index];

        while (current != null)
        {
            if (current.Id == id)
                return current.Node;

            current = current.Next;
        }

        return null;
    }

    // Silme
    public void Delete(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        int index = HashFunc(id);
        HashEntry current = buckets[index];
        HashEntry prev = null;

        while (current != null)
        {
            if (current.Id == id)
            {
                if (prev == null)
                {
                    buckets[index] = current.Next;
                }
                else
                {
                    prev.Next = current.Next;
                }

                Count--;
                return;
            }

            prev = current;
            current = current.Next;
        }
    }
}