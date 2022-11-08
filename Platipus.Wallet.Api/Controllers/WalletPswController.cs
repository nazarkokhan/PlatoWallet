// ReSharper disable UnusedParameter.Global

namespace Platipus.Wallet.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Application.Requests.Wallets.Psw;
using Application.Requests.Wallets.Psw.Base.Response;
using Abstract;
using Extensions;
using StartupSettings.Filters;

[Route("wallet/psw")]
[MockedErrorActionFilter(Order = 1)]
[PswVerifySignatureFilter(Order = 2)]
public class WalletPswController : ApiController
{
    private readonly IMediator _mediator;

    public WalletPswController(IMediator mediator) => _mediator = mediator;

    [HttpPost("balance")]
    [ProducesResponseType(typeof(PswBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        [FromHeader(Name = PswHeaders.XRequestSign)]
        string sign,
        GetBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(PswBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        [FromHeader(Name = PswHeaders.XRequestSign)]
        string sign,
        BetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("win")]
    [ProducesResponseType(typeof(PswBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        [FromHeader(Name = PswHeaders.XRequestSign)]
        string sign,
        WinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("rollback")]
    [ProducesResponseType(typeof(PswBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Rollback(
        [FromHeader(Name = PswHeaders.XRequestSign)]
        string sign,
        RollbackRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("award")]
    [ProducesResponseType(typeof(PswBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Award(
        [FromHeader(Name = PswHeaders.XRequestSign)]
        string sign,
        AwardRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}