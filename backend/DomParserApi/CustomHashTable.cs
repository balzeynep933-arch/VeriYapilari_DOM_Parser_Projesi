using System;
using DomParserApi.Models;

namespace DomParserApi.DataStructures
{
    internal class HashEntry
    {
        public string Key { get; set; }
        public HtmlNode Value { get; set; }
        public HashEntry Next { get; set; }
    }

    public class CustomHashTable
    {
        private readonly int _capacity;
        private HashEntry[] _buckets;

        public CustomHashTable(int capacity = 100)
        {
            _capacity = capacity;
            _buckets = new HashEntry[_capacity];
        }

        private int GetHash(string key)
        {
            int hash = 0;
            for (int i = 0; i < key.Length; i++)
            {
                hash = (hash * 31 + key[i]) % _capacity;
            }
            return Math.Abs(hash);
        }

        public void Insert(string key, HtmlNode node)
        {
            if (string.IsNullOrEmpty(key)) return;

            int index = GetHash(key);
            HashEntry newEntry = new HashEntry { Key = key, Value = node, Next = null };

            if (_buckets[index] == null)
            {
                _buckets[index] = newEntry;
            }
            else
            {
                // Collision handling
                HashEntry current = _buckets[index];
                while (current.Next != null)
                {
                    if (current.Key == key)
                    {
                        current.Value = node; // Update if exists
                        return;
                    }
                    current = current.Next;
                }
                current.Next = newEntry;
            }
        }

        public HtmlNode Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            int index = GetHash(key);
            HashEntry current = _buckets[index];

            while (current != null)
            {
                if (current.Key == key) return current.Value;
                current = current.Next;
            }
            return null; 
        }
    }
}