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
}
