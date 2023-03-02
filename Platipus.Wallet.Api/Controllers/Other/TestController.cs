namespace Platipus.Wallet.Api.Controllers.Other;

using System.Text.Json;
using System.Text.Json.Nodes;
using Abstract;
using Microsoft.AspNetCore.Mvc;

[Route("test")]
public class TestController : RestApiController
{
    [HttpPost("stringify")]
    public IActionResult Stringify([FromBody] JsonNode request)
    {
        return Ok(JsonSerializer.Serialize(request));
    }

    [HttpGet("openbox/unix-now")]
    public IActionResult OpenboxUnixNow(DateTime? time)
    {
        var unixNow = (time ?? DateTimeOffset.UtcNow).ToUnixTimeMilliseconds();

        var result = new { UnixNow = unixNow };

        return Ok(result);
    }
}