using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DomParserApi.Models;
using DomParserApi.DataStructures;

namespace DomParserApi.Parser
{
    public class DomSearcher
    {
        private readonly HtmlNode _root;
        private readonly CustomHashTable _idTable;

        public DomSearcher(HtmlNode root)
        {
            _root = root;
            _idTable = new CustomHashTable(200);
            BuildIndex();
        }

        // Build the Hash Table by traversing the tree once (using BFS)
        private void BuildIndex()
        {
            if (_root == null) return;

            CustomQueue queue = new CustomQueue();
            queue.Enqueue(_root);

            while (!queue.IsEmpty())
            {
                HtmlNode current = queue.Dequeue();

                if (!string.IsNullOrEmpty(current.Id))
                {
                    _idTable.Insert(current.Id, current);
                }

                if (current.Children != null)
                {
                    foreach (var child in current.Children)
                    {
                        if (child != null) queue.Enqueue(child);
                    }
                }
            }
        }

        public List<HtmlNode> Search(string query)
        {
            List<HtmlNode> results = new List<HtmlNode>();
            if (string.IsNullOrWhiteSpace(query)) return results;

            var idMatch = Regex.Match(query, @"id\s*=\s*[""']?([\w-]+)[""']?");
            var classMatch = Regex.Match(query, @"class\s*=\s*[""']?([\w-]+)[""']?");

            // O(1) Search using Hash Table
            if (idMatch.Success)
            {
                Console.WriteLine("Backend Search: O(1) Hash Table lookup for ID " + idMatch.Groups[1].Value);
                HtmlNode node = _idTable.Get(idMatch.Groups[1].Value);
                if (node != null) results.Add(node);
                return results;
            }

            // O(N) Search using BFS
            Console.WriteLine("Backend Search: O(N) BFS traversal for query " + query);
            
            CustomQueue queue = new CustomQueue();
            if (_root != null) queue.Enqueue(_root);

            while (!queue.IsEmpty())
            {
                HtmlNode current = queue.Dequeue();
                bool hit = false;

                if (classMatch.Success)
                {
                    string targetClass = classMatch.Groups[1].Value;
                    if (!string.IsNullOrEmpty(current.ClassName) && current.ClassName.Contains(targetClass))
                    {
                        hit = true;
                    }
                }
                else
                {
                    // Plain text match
                    string fullTag = $"<{current.Tag} {(!string.IsNullOrEmpty(current.Id) ? $"id=\"{current.Id}\"" : "")} {(!string.IsNullOrEmpty(current.ClassName) ? $"class=\"{current.ClassName}\"" : "")}>";
                    if ((current.Tag != null && current.Tag.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (current.Id != null && current.Id.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (current.ClassName != null && current.ClassName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (current.Text != null && current.Text.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        fullTag.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        hit = true;
                    }
                }

                if (hit) results.Add(current);

                if (current.Children != null)
                {
                    foreach (var child in current.Children)
                    {
                        if (child != null) queue.Enqueue(child);
                    }
                }
            }

            return results;
        }
    }
}
