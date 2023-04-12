namespace Platipus.Wallet.Api.Controllers.Other;

using System.Text.Json;
using System.Text.Json.Nodes;
using Abstract;
using Microsoft.AspNetCore.Mvc;

[Route("test")]
public class TestController : RestApiController
{
    [HttpPost("stringify-json")]
    public IActionResult Stringify([FromBody] JsonNode request)
    {
        var jsonText = request.ToJsonString();
        return Ok(jsonText);
    }

    [HttpGet("de-stringify-json")]
    public IActionResult Stringify(string jsonString)
    {
        var jsonNode = JsonDocument.Parse(jsonString.Replace("\\\"", "\""));
        return Ok(jsonNode);
    }

    [HttpGet("openbox/unix-now")]
    public IActionResult OpenboxUnixNow(DateTime? time)
    {
        var unixNow = (time ?? DateTimeOffset.UtcNow).ToUnixTimeMilliseconds();

        var result = new { UnixNow = unixNow };

        return Ok(result);
    }
}