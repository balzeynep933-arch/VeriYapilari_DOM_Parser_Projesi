using System;
using System.Collections.Generic;
using DomParserAPI.Models; // Arkadaşlarının Models klasöründeki HtmlNode'u görmek için

namespace DomParserAPI.Logic
{
    public class TreeAnalyzer
    {
        // 1. BFS (Breadth-First Search) - Genişlik Öncelikli Seviye Taraması
        public List<HtmlNode> BFS(HtmlNode root)
        {
            List<HtmlNode> resultList = new List<HtmlNode>();
            if (root == null) return resultList;
            
            MyQueue<HtmlNode> queue = new MyQueue<HtmlNode>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                HtmlNode current = queue.Dequeue();
                resultList.Add(current);

                if (current.Children != null)
                {
                    foreach (var child in current.Children)
                    {
                        queue.Enqueue(child);
                    }
                }
            }
            return resultList;
        }

        // 2. DFS (Depth-First Search) - Derinlik Öncelikli Rekürsif Tarama
        public List<HtmlNode> DFS(HtmlNode root)
        {
            List<HtmlNode> resultList = new List<HtmlNode>();
            DFSHelper(root, resultList);
            return resultList;
        }

        private void DFSHelper(HtmlNode node, List<HtmlNode> list)
        {
            if (node == null) return;
            
            list.Add(node);
            
            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    DFSHelper(child, list);
                }
            }
        }

        // 3. Ağaç Derinliği Hesaplama (Tree Depth)
        public int CalculateDepth(HtmlNode node)
        {
            if (node == null) return 0;
            if (node.Children == null || node.Children.Count == 0) return 1;

            int maxChildDepth = 0;
            foreach (var child in node.Children)
            {
                int currentDepth = CalculateDepth(child);
                if (currentDepth > maxChildDepth)
                {
                    maxChildDepth = currentDepth;
                }
            }
            return 1 + maxChildDepth;
        }

        // 4. Kardeş Düğümleri Bulma (Get Siblings)
        public List<HtmlNode> GetSiblings(HtmlNode node)
        {
            List<HtmlNode> siblings = new List<HtmlNode>();
            if (node == null || node.Parent == null) return siblings;

            if (node.Parent.Children != null)
            {
                foreach (var child in node.Parent.Children)
                {
                    if (child != node)
                    {
                        siblings.Add(child);
                    }
                }
            }
            return siblings;
        }

        // 5. Alt Ağaç Analizi (Subtree Analysis)
        public int AnalyzeSubtreeCount(HtmlNode subRoot)
        {
            if (subRoot == null) return 0;
            
            int totalNodes = 1;
            if (subRoot.Children != null)
            {
                foreach (var child in subRoot.Children)
                {
                    totalNodes += AnalyzeSubtreeCount(child);
                }
            }
            return totalNodes;
        }
    }
}