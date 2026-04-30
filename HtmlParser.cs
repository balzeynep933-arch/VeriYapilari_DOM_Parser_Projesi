using System;
using DomParserProject.Core;
using DomParserProject.DataStructures;

namespace DomParserProject.Parser
{
    public class HtmlParser
    {
        public HtmlNode ParseToTree(string htmlText)
        {
            if (string.IsNullOrWhiteSpace(htmlText)) return null;

            CustomStack stack = new CustomStack();
            // Tüm DOM'u tutacak görünmez bir kök (root) düğüm oluşturuyoruz
            HtmlNode root = new HtmlNode("document"); 
            stack.Push(root);

            int index = 0;
            while (index < htmlText.Length)
            {
                int openBracketIndex = htmlText.IndexOf('<', index);
                if (openBracketIndex == -1) break; // İşlenecek etiket kalmadı

                int closeBracketIndex = htmlText.IndexOf('>', openBracketIndex);
                if (closeBracketIndex == -1) break; // Hatalı HTML yapısı

                // '<' ve '>' arasındaki metni al (örn: "div id='main'" veya "/div")
                string tagContent = htmlText.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1).Trim();
                
                if (tagContent.StartsWith("/")) // Kapanış Etiketi (örn: </p>)
                {
                    stack.Pop(); // Ağaçta bir üst seviyeye (ebeveyne) geri dön
                }
                else // Açılış Etiketi (örn: <p class="text">)
                {
                    // Tag içeriğinden Düğüm objesi üret
                    HtmlNode newNode = ParseTagContent(tagContent);
                    
                    // Yığıtın en üstündeki düğüm, yeni düğümün ebeveynidir
                    HtmlNode parent = stack.Peek();
                    if (parent != null)
                    {
                        parent.AddChild(newNode);
                    }

                    // Eğer kendi kendine kapanan (self-closing) bir etiket değilse, yığıta ekle
                    // Örn: <img /> yığıta girmez, ama <div> yığıta girer.
                    if (!tagContent.EndsWith("/"))
                    {
                        stack.Push(newNode);
                    }
                }

                // Bir sonraki etiketi aramak için indeksi güncelle
                index = closeBracketIndex + 1;
            }

            return root; // Oluşturulan DOM ağacının başlangıç noktasını döndür
        }

        // Yardımcı Metod: HTML etiketi içindeki "id" ve "class" değerlerini ayıklar
        private HtmlNode ParseTagContent(string content)
        {
            // Örnek content: "div id=\"header\" class=\"container\""
            string[] parts = content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string tagName = parts[0]; // İlk parça her zaman tag adıdır
            
            string id = "";
            string className = "";

            for (int i = 1; i < parts.Length; i++)
            {
                if (parts[i].StartsWith("id="))
                    id = ExtractAttribute(parts[i]);
                else if (parts[i].StartsWith("class="))
                    className = ExtractAttribute(parts[i]);
            }

            return new HtmlNode(tagName, id, className);
        }

        // Yardımcı Metod: Tırnak işaretleri arasındaki değeri okur
        private string ExtractAttribute(string attributeString)
        {
            int firstQuote = attributeString.IndexOf('"');
            int lastQuote = attributeString.LastIndexOf('"');
            
            if (firstQuote != -1 && lastQuote != -1 && firstQuote < lastQuote)
            {
                return attributeString.Substring(firstQuote + 1, lastQuote - firstQuote - 1);
            }
            return "";
        }
    }
}