// ReSharper disable UnusedParameter.Global

namespace PlatipusWallet.Api.Controllers.Wallets;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Requests.Base.Responses;
using Application.Requests.Wallet;
using Abstract;
using Extensions;
using Filters;
using StartupSettings;

[Route("wallet/psw")]
[MockedErrorActionFilter(Order = 1)]
[PswVerifySignatureFilter(Order = 2)]
public class WalletPswController : ApiController
{
    private readonly IMediator _mediator;

    public WalletPswController(IMediator mediator) => _mediator = mediator;

    [HttpPost("balance")]
    [ProducesResponseType(typeof(BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        [FromHeader(Name = PswHeaders.XRequestSign)]
        string sign,
        GetBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        [FromHeader(Name = PswHeaders.XRequestSign)]
        string sign,
        BetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("win")]
    [ProducesResponseType(typeof(BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        [FromHeader(Name = PswHeaders.XRequestSign)]
        string sign,
        WinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("rollback")]
    [ProducesResponseType(typeof(BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Rollback(
        [FromHeader(Name = PswHeaders.XRequestSign)]
        string sign,
        RollbackRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("award")]
    [ProducesResponseType(typeof(BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Award(
        [FromHeader(Name = PswHeaders.XRequestSign)]
        string sign,
        AwardRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}