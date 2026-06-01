using System.Text.Json.Serialization;

namespace DomParserAPI.Models;

public class DomNode
{
    // Etiket adı veya "text"
    public string TagName { get; set; } = string.Empty;
    
    // Eğer bu bir metin düğümü ise içeriğini burada tutacağız
    public string TextContent { get; set; } = string.Empty;
    
    // Etiketin sahip olduğu özellikler 
    public Dictionary<string, string> Attributes { get; set; } = new();
    
    // N-ary Tree: Bir düğümün birden fazla alt düğümü (çocuğu) olabilir
    public List<DomNode> Children { get; set; } = new();
    
    // Hiyerarşiyi geriye doğru takip edebilmek için ebeveyn düğüm
    // [JsonIgnore] çok önemlidir! Aksi halde API JSON dönerken parent-child arasında sonsuz döngüye girer.
    [JsonIgnore] 
    public DomNode? Parent { get; set; }
}