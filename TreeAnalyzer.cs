using System;
using System.Collections.Generic;

namespace VeriYapilariProje
{
    public class TreeAnalyzer
    {
        // 1. BFS - Genişlik Öncelikli Arama (Level-order Traversal)
        public void BFS(HtmlNode root)
        {
            if (root == null) return;
            
            MyQueue<HtmlNode> queue = new MyQueue<HtmlNode>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                HtmlNode current = queue.Dequeue();
                
                // İşlem: Düğüm bilgilerini ekrana yazdır (veya bir listede topla)
                Console.WriteLine($"Etiket: {current.TagName} | ID: {current.Id}");

                foreach (var child in current.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        // 2. DFS - Derinlik Öncelikli Arama (Recursive)
        public void DFS(HtmlNode node)
        {
            if (node == null) return;

            Console.WriteLine($"Etiket: {node.TagName}");

            foreach (var child in node.Children)
            {
                DFS(child);
            }
        }

        // 3. Ağaç Derinliği Hesaplama (Rekürsif)
        public int GetTreeDepth(HtmlNode node)
        {
            if (node == null || node.Children.Count == 0) return 0;

            int maxDepth = 0;
            foreach (var child in node.Children)
            {
                int currentDepth = GetTreeDepth(child);
                if (currentDepth > maxDepth) maxDepth = currentDepth;
            }
            return 1 + maxDepth;
        }

        // 4. Kardeş Düğümleri Bulma
        public List<HtmlNode> GetSiblings(HtmlNode node)
        {
            List<HtmlNode> siblings = new List<HtmlNode>();
            if (node == null || node.Parent == null) return siblings;

            foreach (var child in node.Parent.Children)
            {
                if (child != node) siblings.Add(child);
            }
            return siblings;
        }
    }
}