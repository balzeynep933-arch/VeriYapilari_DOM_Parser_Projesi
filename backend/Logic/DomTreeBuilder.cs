using DomParserAPI.Models;

namespace DomParserAPI.Logic;

public class DomTreeBuilder
{
    public DomNode BuildTree(List<Token> tokens)
    {
        // Ağacın kökünü temsil edecek sanal bir "root" düğümü oluşturuyoruz.
        var root = new DomNode { TagName = "root" };
        var stack = new CustomStack<DomNode>();
        
        stack.Push(root); // Kök düğümü yığına at

        foreach (var token in tokens)
        {
            if (token.Type == TokenType.OpenTag)
            {
                // 1. Yeni düğüm oluştur
                var newNode = new DomNode
                {
                    TagName = token.Value,
                    Attributes = token.Attributes,
                    Parent = stack.Peek() // Ebeveyni, yığının en üstündeki düğümdür
                };
                
                // 2. Yeni düğümü ebeveyninin çocukları arasına ekle
                stack.Peek().Children.Add(newNode);
                
                // 3. İçine başka etiketler gelebileceği için yeni düğümü yığına ekle
                stack.Push(newNode);
            }
            else if (token.Type == TokenType.CloseTag)
            {
                // Kapanış etiketi geldiğinde, yığının en üstündeki etiket ile eşleşiyorsa yığından çıkar
                if (!stack.IsEmpty() && stack.Peek().TagName == token.Value)
                {
                    stack.Pop();
                }
            }
            else if (token.Type == TokenType.SelfClosingTag)
            {
                var newNode = new DomNode
                {
                    TagName = token.Value,
                    Attributes = token.Attributes,
                    Parent = stack.Peek()
                };
                stack.Peek().Children.Add(newNode);
            }
            else if (token.Type == TokenType.Text)
            {
                // Sadece metin içeriği.
                var textNode = new DomNode
                {
                    TagName = "text",
                    TextContent = token.Value,
                    Parent = stack.Peek()
                };
                stack.Peek().Children.Add(textNode);
            }
        }

        return root; // Oluşan tam ağacı döndür
    }
}