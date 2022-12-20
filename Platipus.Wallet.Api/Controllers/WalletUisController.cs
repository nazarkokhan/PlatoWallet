namespace Platipus.Wallet.Api.Controllers;

using System.Net.Mime;
using Abstract;
using Application.Requests.Wallets.Uis;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.Filters;

[Route("wallet/uis")]
[MockedErrorActionFilter(Order = 1)]
// [Hub88VerifySignatureFilter(Order = 2)]
[Produces(MediaTypeNames.Application.Xml)]
[Consumes(MediaTypeNames.Application.Xml)]
public class WalletUisController : ApiController
{
    private readonly IMediator _mediator;

    public WalletUisController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate(
        [FromBody] UisAuthenticateRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("change-balance")]
    public async Task<IActionResult> ChangeBalance(
        [FromBody] UisChangeBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("status")]
    public async Task<IActionResult> Status(
        [FromBody] UisStatusRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("get-balance")]
    public async Task<IActionResult> GetBalance(
        [FromBody] UisGetBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}