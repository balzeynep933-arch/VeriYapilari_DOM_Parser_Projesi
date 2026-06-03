using Microsoft.AspNetCore.Mvc;
using DomParserApi.Models;
using DomParserApi.Parser;

namespace DomParserApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParseController : ControllerBase
{
    [HttpPost]
    public ActionResult<HtmlNode> Post([FromBody] string html)
    {
        var parser = new HtmlParser();
        var root = parser.ParseToTree(html);
        return Ok(root);
    }

    [HttpPost("search")]
    public ActionResult<List<HtmlNode>> Search([FromBody] SearchRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Html))
            return BadRequest("Html content is missing.");

        var parser = new HtmlParser();
        var root = parser.ParseToTree(request.Html);
        
        var searcher = new DomSearcher(root);
        var results = searcher.Search(request.Query);
        
        return Ok(results);
    }
}
