namespace Platipus.Wallet.Api.Controllers;

using System.Net.Mime;
using Abstract;
using Application.Requests.Wallets.Uis;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.Filters;

[Route("wallet/uis")]
[MockedErrorActionFilter(Order = 1)]
//TODO [UisVerifySignatureFilter(Order = 0)]
[Produces(MediaTypeNames.Application.Xml)]
[Consumes(MediaTypeNames.Application.Xml)]
public class WalletUisController : ApiController
{
    private readonly IMediator _mediator;

    public WalletUisController(IMediator mediator)
    {
        _mediator = mediator;
    }

    //redeploy
    [HttpGet("authenticate")]
    public async Task<IActionResult> Authenticate(
        [FromQuery] UisAuthenticateRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("change-balance")]
    public async Task<IActionResult> ChangeBalance(
        [FromQuery] UisChangeBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("status")]
    public async Task<IActionResult> Status(
        [FromQuery] UisStatusRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("get-balance")]
    public async Task<IActionResult> GetBalance(
        [FromQuery] UisGetBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}