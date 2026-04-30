using DomParserAPI.Models;
using System.Text.RegularExpressions;

namespace DomParserAPI.Logic;

public class HtmlTokenizer
{
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

                string fullTag = html.Substring(i + 1, end - i - 1);
                var token = new Token();

                if (fullTag.StartsWith("/"))
                {
                    token.Type = TokenType.CloseTag;
                    token.Value = fullTag.Substring(1).Trim();
                }
                else
                {
                    token.Type = TokenType.OpenTag;
                    var parts = Regex.Matches(fullTag, @"[\w-]+|""[^""]*""");
                    token.Value = parts[0].Value;

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
                tokens.Add(token);
                i = end + 1;
            }
            else
            {
                int nextTag = html.IndexOf('<', i);
                if (nextTag == -1) nextTag = html.Length;

                string text = html.Substring(i, nextTag - i).Trim();
                if (!string.IsNullOrEmpty(text))
                    tokens.Add(new Token { Type = TokenType.Text, Value = text });

                i = nextTag;
            }
        }
        return tokens;
    }
}