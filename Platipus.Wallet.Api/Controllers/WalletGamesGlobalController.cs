namespace Platipus.Wallet.Api.Controllers;

using System.Net.Mime;
using System.Xml;
using Microsoft.AspNetCore.Mvc;

[Route("wallet/games-global")]
[Produces(MediaTypeNames.Application.Xml)]
[Consumes(MediaTypeNames.Application.Xml)]
[Obsolete]
//TODO remove all GamesGlobalCode
public class WalletGamesGlobalController : Controller
{
    [HttpPost]
    public IActionResult Regular(
        [FromHeader(Name = "UserName")] string username,
        [FromHeader(Name = "Password")] string password,
        [FromHeader(Name = "RequestId")] Guid requestId,
        [FromBody] XmlDocument request,
        CancellationToken cancellationToken)
    {
        throw new Exception("Is not accessible point");
    }

    [HttpPost("admin")]
    public IActionResult Admin(
        [FromHeader(Name = "UserName")] string username,
        [FromHeader(Name = "Password")] string password,
        [FromHeader(Name = "RequestId")] Guid requestId,
        [FromBody] XmlDocument request,
        CancellationToken cancellationToken)
    {
        throw new Exception("Is not accessible point");
    }
}