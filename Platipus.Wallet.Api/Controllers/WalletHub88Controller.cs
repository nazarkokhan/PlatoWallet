// ReSharper disable UnusedParameter.Global

namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.Psw;
using Application.Requests.Wallets.Psw.Base.Response;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.Filters;

[Route("wallet/hub88")]
[MockedErrorActionFilter(Order = 1)]
[Hub88VerifySignatureFilter(Order = 2)]
public class WalletHub88Controller : ApiController
{
    private readonly IMediator _mediator;

    public WalletHub88Controller(IMediator mediator)
        => _mediator = mediator;

    [HttpPost("balance")]
    [ProducesResponseType(typeof(PswBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        [FromHeader(Name = Hub88Headers.XHub88Signature)] string sign,
        PswGetBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(PswBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        [FromHeader(Name = Hub88Headers.XHub88Signature)] string sign,
        PswBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("win")]
    [ProducesResponseType(typeof(PswBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        [FromHeader(Name = Hub88Headers.XHub88Signature)] string sign,
        PswWinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("rollback")]
    [ProducesResponseType(typeof(PswBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Rollback(
        [FromHeader(Name = Hub88Headers.XHub88Signature)] string sign,
        PswRollbackRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}