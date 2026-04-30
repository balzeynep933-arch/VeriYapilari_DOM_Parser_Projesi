namespace DomParserAPI.Models;

public enum TokenType { OpenTag, CloseTag, Text }

public class Token
{
    public TokenType Type { get; set; }
    public string Value { get; set; } = string.Empty;
    public Dictionary<string, string> Attributes { get; set; } = new();
}