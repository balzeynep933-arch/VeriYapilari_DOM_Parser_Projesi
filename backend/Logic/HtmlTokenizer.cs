using System.Text.RegularExpressions;
using DomParserAPI.Models;

namespace DomParserAPI.Logic;

public class HtmlTokenizer
{
    private readonly HashSet<string> _voidElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "area", "base", "br", "col", "embed", "hr", "img", "input", 
        "link", "meta", "source", "track", "wbr"
    };

    public List<Token> Tokenize(string html)
    {
        var tokens = new List<Token>();
        int i = 0;

        while (i < html.Length)
        {
            if (html[i] == '<')
            {
                int end = html.IndexOf('>', i);
                if (end == -1) break;

                string fullTag = html.Substring(i + 1, end - i - 1).Trim();
                var token = new Token();

                if (fullTag.StartsWith("/"))
                {
                    token.Type = TokenType.CloseTag;
                    token.Value = fullTag.Substring(1).Trim();
                }
                else
                {
                    bool isSelfClosing = fullTag.EndsWith("/");
                    if (isSelfClosing)
                    {
                        fullTag = fullTag.Substring(0, fullTag.Length - 1).Trim();
                    }

                    var parts = Regex.Matches(fullTag, @"[\w-]+|""[^""]*""");
                    if (parts.Count > 0)
                    {
                        token.Value = parts[0].Value;

                        if (!isSelfClosing && _voidElements.Contains(token.Value))
                        {
                            isSelfClosing = true;
                        }

                        token.Type = isSelfClosing ? TokenType.SelfClosingTag : TokenType.OpenTag;

                        for (int j = 1; j < parts.Count; j += 2)
                        {
                            if (j + 1 < parts.Count)
                            {
                                string key = parts[j].Value;
                                string val = parts[j + 1].Value.Trim('"');
                                token.Attributes[key] = val;
                            }
                        }
                    }
                }
                tokens.Add(token);
                i = end + 1;
            }
            else
            {
                int nextTag = html.IndexOf('<', i);
                if (nextTag == -1) nextTag = html.Length;

                string text = html.Substring(i, nextTag - i).Trim();
                if (!string.IsNullOrEmpty(text))
                {
                    tokens.Add(new Token { Type = TokenType.Text, Value = text });
                }

                i = nextTag;
            }
        }
        return tokens;
    }
}