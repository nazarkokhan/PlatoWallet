namespace Platipus.Wallet.Api.Controllers.Other;

using System.Text.Json;
using Abstract;
using Microsoft.AspNetCore.Mvc;

[Route("test")]
public class TestController : RestApiController
{
    [HttpPost("stringify-json")]
    public IActionResult Stringify([FromBody] JsonDocument request)
    {
        var jsonText = request.RootElement.ToString();
        return Ok(jsonText);
    }

    [HttpGet("de-stringify-json")]
    public IActionResult Stringify(string jsonString)
    {
        var jsonDocument = JsonDocument.Parse(jsonString.Replace("\\\"", "\""));
        return Ok(jsonDocument);
    }

    [HttpGet("openbox/unix-now")]
    public IActionResult OpenboxUnixNow(DateTime? time)
    {
        var unixNow = (time ?? DateTimeOffset.UtcNow).ToUnixTimeMilliseconds();

        var result = new { UnixNow = unixNow };

        return Ok(result);
    }
}