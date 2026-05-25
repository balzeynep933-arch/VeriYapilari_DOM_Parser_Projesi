using Microsoft.AspNetCore.Mvc;
using DomParserAPI.Logic;
using DomParserAPI.Models;

namespace DomParserAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParserController : ControllerBase
{
    private readonly HtmlTokenizer _tokenizer;
    private readonly DomTreeBuilder _treeBuilder; // YENİ

    // Constructor güncellendi
    public ParserController(HtmlTokenizer tokenizer, DomTreeBuilder treeBuilder)
    {
        _tokenizer = tokenizer;
        _treeBuilder = treeBuilder;
    }

    [HttpPost("tokenize")]
    public ActionResult<List<Token>> Post([FromBody] string htmlInput)
    {
        if (string.IsNullOrEmpty(htmlInput)) 
            return BadRequest("Input cannot be empty.");

        var result = _tokenizer.Tokenize(htmlInput);
        return Ok(result);
    }

    // YENİ ENDPOINT: Ağacı döndürür
    [HttpPost("build-tree")]
    public ActionResult<DomNode> BuildTree([FromBody] string htmlInput)
    {
        if (string.IsNullOrEmpty(htmlInput)) 
            return BadRequest("Input cannot be empty.");

        // 1. Önce token'lara ayır (1. Kişinin görevi)
        var tokens = _tokenizer.Tokenize(htmlInput);
        
        // 2. Token'lardan ağaç inşa et (2. Kişinin görevi)
        var tree = _treeBuilder.BuildTree(tokens);
        
        return Ok(tree);
    }
}