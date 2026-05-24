using Microsoft.AspNetCore.Mvc;
using DomParserAPI.Logic;
using DomParserAPI.Models;

namespace DomParserAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParserController : ControllerBase
{
    private readonly HtmlTokenizer _tokenizer;

    public ParserController(HtmlTokenizer tokenizer)
    {
        _tokenizer = tokenizer;
    }

    [HttpPost("tokenize")]
    public ActionResult<List<Token>> Post([FromBody] string htmlInput)
    {
        if (string.IsNullOrEmpty(htmlInput)) 
            return BadRequest("Input cannot be empty.");

        var result = _tokenizer.Tokenize(htmlInput);
        return Ok(result);
    }
}