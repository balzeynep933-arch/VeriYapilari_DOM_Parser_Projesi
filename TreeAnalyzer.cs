using System;
using System.Collections.Generic;
using DomParserAPI.Models; // Arkadaşlarının DomNode yapısını görmesi için

namespace DomParserAPI.Logic
{
    public class TreeAnalyzer
    {
        // 1. BFS (Breadth-First Search) - Genişlik Öncelikli Seviye Taraması
        public List<DomNode> BFS(DomNode root)
        {
            List<DomNode> resultList = new List<DomNode>();
            if (root == null) return resultList;
            
            MyQueue<DomNode> queue = new MyQueue<DomNode>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                DomNode current = queue.Dequeue();
                resultList.Add(current);

                if (current.Children != null)
                {
                    foreach (var child in current.Children)
                    {
                        if (child != null) queue.Enqueue(child);
                    }
                }
            }
            return resultList;
        }

        // 2. DFS (Depth-First Search) - Derinlik Öncelikli Rekürsif Tarama
        public List<DomNode> DFS(DomNode root)
        {
            List<DomNode> resultList = new List<DomNode>();
            DFSHelper(root, resultList);
            return resultList;
        }

        private void DFSHelper(DomNode node, List<DomNode> list)
        {
            if (node == null) return;
            
            list.Add(node);
            
            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    if (child != null) DFSHelper(child, list);
                }
            }
        }

        // 3. Ek Fonksiyon: Ağaç Derinliği Hesaplama (Tree Depth)
        public int CalculateDepth(DomNode node)
        {
            if (node == null) return 0;
            if (node.Children == null || node.Children.Count == 0) return 1;

            int maxChildDepth = 0;
            foreach (var child in node.Children)
            {
                if (child != null)
                {
                    int currentDepth = CalculateDepth(child);
                    if (currentDepth > maxChildDepth)
                    {
                        maxChildDepth = currentDepth;
                    }
                }
            }
            return 1 + maxChildDepth;
        }

        // 4. Ek Fonksiyon: Kardeş Düğümleri Bulma (Get Siblings)
        public List<DomNode> GetSiblings(DomNode node)
        {
            List<DomNode> realSiblings = new List<DomNode>();
            if (node == null || node.Parent == null) return realSiblings;

            if (node.Parent.Children != null)
            {
                foreach (var child in node.Parent.Children)
                {
                    if (child != null && child != node)
                    {
                        realSiblings.Add(child);
                    }
                }
            }
            return realSiblings;
        }

        // 5. Ek Fonksiyon: Alt Ağaç Analizi (Subtree Analysis)
        public int AnalyzeSubtreeCount(DomNode subRoot)
        {
            if (subRoot == null) return 0;
            
            int totalNodes = 1; // Kendisini sayıyoruz
            if (subRoot.Children != null)
            {
                foreach (var child in subRoot.Children)
                {
                    if (child != null)
                    {
                        totalNodes += AnalyzeSubtreeCount(child);
                    }
                }
            }
            return totalNodes;
        }
    }
}
