using System;
using System.Text.RegularExpressions;
using DomParserApi.Models;
using DomParserApi.DataStructures;

namespace DomParserApi.Parser
{
    public class HtmlParser
    {
        public HtmlNode ParseToTree(string htmlText)
        {
            if (string.IsNullOrWhiteSpace(htmlText)) return null;

            CustomStack stack = new CustomStack();
            // Create an invisible root node to hold the entire DOM
            HtmlNode root = new HtmlNode("document"); 
            stack.Push(root);

            int index = 0;
            while (index < htmlText.Length)
            {
                int openBracketIndex = htmlText.IndexOf('<', index);
                
                // 1. Extract text nodes between tags
                if (openBracketIndex == -1) 
                {
                    AddTextNode(stack, htmlText.Substring(index));
                    break;
                }
                
                if (openBracketIndex > index)
                {
                    AddTextNode(stack, htmlText.Substring(index, openBracketIndex - index));
                }

                int closeBracketIndex = htmlText.IndexOf('>', openBracketIndex);
                if (closeBracketIndex == -1) break; // Malformed HTML

                string tagContent = htmlText.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1).Trim();
                
                // Skip comments and doctype
                if (tagContent.StartsWith("!") || tagContent.StartsWith("?"))
                {
                    index = closeBracketIndex + 1;
                    continue;
                }

                if (tagContent.StartsWith("/")) // Closing Tag (e.g., </p>)
                {
                    if (!stack.IsEmpty()) stack.Pop();
                }
                else // Opening Tag (e.g., <p class="text">)
                {
                    HtmlNode newNode = ParseTagContent(tagContent);
                    
                    HtmlNode parent = stack.Peek();
                    if (parent != null)
                    {
                        parent.AddChild(newNode);
                    }

                    // Check if it's a self-closing tag
                    bool isSelfClosing = tagContent.EndsWith("/") || IsVoidElement(newNode.Tag);
                    if (!isSelfClosing)
                    {
                        stack.Push(newNode);
                    }
                }

                index = closeBracketIndex + 1;
            }

            return root;
        }

        private void AddTextNode(CustomStack stack, string text)
        {
            text = text.Replace("\n", " ").Replace("\r", " ").Trim();
            if (!string.IsNullOrWhiteSpace(text))
            {
                HtmlNode textNode = new HtmlNode("#text") { Text = text };
                stack.Peek()?.AddChild(textNode);
            }
        }

        private bool IsVoidElement(string tag)
        {
            string[] voidElements = { "br", "hr", "img", "input", "link", "meta", "area", "base", "col", "embed", "param", "source", "track", "wbr" };
            return Array.Exists(voidElements, e => e.Equals(tag, StringComparison.OrdinalIgnoreCase));
        }

        private HtmlNode ParseTagContent(string content)
        {
            // Extract tag name using Regex (first word)
            var tagMatch = Regex.Match(content, @"^([a-zA-Z0-9]+)");
            string tagName = tagMatch.Success ? tagMatch.Groups[1].Value : "unknown";
            
            // Extract id and class using Regex to support spaces in values
            string id = ExtractAttribute(content, "id");
            string className = ExtractAttribute(content, "class");

            return new HtmlNode(tagName, id, className);
        }

        private string ExtractAttribute(string content, string attrName)
        {
            // Regex to match attribute="..." or attribute='...'
            var match = Regex.Match(content, attrName + @"\s*=\s*[""']([^""']*)[""']", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : "";
        }
    }
}
