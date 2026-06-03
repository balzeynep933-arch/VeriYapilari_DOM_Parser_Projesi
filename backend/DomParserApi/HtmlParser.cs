using System;
using System.Text.RegularExpressions;
using DomParserApi.Models;
using DomParserApi.DataStructures;

namespace DomParserApi.Parser
{
    public class HtmlParser
    {
        private int _nodeIndex = 0;

        public HtmlNode ParseToTree(string htmlText)
        {
            _nodeIndex = 0;
            if (string.IsNullOrWhiteSpace(htmlText)) return null;

            CustomStack stack = new CustomStack();
            // Create an invisible root node to hold the entire DOM
            HtmlNode root = new HtmlNode("document") { Index = _nodeIndex++ }; 
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

                    // Auto-close missing tags (li, td, th, tr)
                    string tLow = newNode.Tag.ToLower();
                    while (!stack.IsEmpty() && stack.Peek().Tag != "document")
                    {
                        string pTag = stack.Peek().Tag.ToLower();
                        bool shouldPop = false;
                        if (tLow == "li" && pTag == "li") shouldPop = true;
                        else if ((tLow == "td" || tLow == "th") && (pTag == "td" || pTag == "th")) shouldPop = true;
                        else if (tLow == "tr" && (pTag == "td" || pTag == "th" || pTag == "tr")) shouldPop = true;
                        
                        if (shouldPop) stack.Pop();
                        else break;
                    }
                    
                    HtmlNode parent = stack.Peek();
                    parent?.AddChild(newNode);

                    // Check if it's a self-closing tag
                    bool isSelfClosing = tagContent.EndsWith("/") || IsVoidElement(newNode.Tag);
                    if (!isSelfClosing)
                    {
                        stack.Push(newNode);

                        // Skip internal content for script/style
                        if (tLow == "script" || tLow == "style")
                        {
                            string closeTag = $"</{newNode.Tag}>";
                            int scriptStartIndex = closeBracketIndex + 1;
                            int closeTagIndex = htmlText.IndexOf(closeTag, scriptStartIndex, StringComparison.OrdinalIgnoreCase);
                            if (closeTagIndex != -1)
                            {
                                string innerScript = htmlText.Substring(scriptStartIndex, closeTagIndex - scriptStartIndex);
                                AddTextNode(stack, innerScript);
                                stack.Pop();
                                index = closeTagIndex + closeTag.Length;
                                continue;
                            }
                        }
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
                HtmlNode textNode = new HtmlNode("#text") { Text = text, Index = _nodeIndex++ };
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
            // Extract tag name using Regex (first word, allowing hyphens)
            var tagMatch = Regex.Match(content, @"^([a-zA-Z0-9\-]+)");
            string tagName = tagMatch.Success ? tagMatch.Groups[1].Value : "unknown";
            
            // Extract id and class using Regex to support spaces in values
            string id = ExtractAttribute(content, "id");
            string className = ExtractAttribute(content, "class");

            return new HtmlNode(tagName, id, className) { Index = _nodeIndex++ };
        }

        private string ExtractAttribute(string content, string attrName)
        {
            // Regex to match attribute="..." or attribute='...'
            var match = Regex.Match(content, attrName + @"\s*=\s*[""']([^""']*)[""']", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : "";
        }
    }
}
