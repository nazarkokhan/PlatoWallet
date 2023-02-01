namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.Betflag;
using Application.Requests.Wallets.Betflag.Base;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;

[Route("wallet/betflag")]
[MockedErrorActionFilter(Order = 1)]
[BetflagVerifyHashFilter(Order = 0)]
[JsonSettingsName(nameof(CasinoProvider.Betflag))]
[ProducesResponseType(typeof(BetflagErrorResponse), StatusCodes.Status400BadRequest)]
public class WalletBetflagController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletBetflagController(IMediator mediator) => _mediator = mediator;

    [HttpPost("balance")]
    [ProducesResponseType(typeof(BetflagBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance(
        BetflagBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(BetflagBetWinCancelResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        BetflagBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("win")]
    [ProducesResponseType(typeof(BetflagBetWinCancelResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        BetflagWinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("cancel")]
    [ProducesResponseType(typeof(BetflagBetWinCancelResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(
        BetflagCancelRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("authenticate")]
    [ProducesResponseType(typeof(BetflagBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Authenticate(
        BetflagAuthenticateRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [Obsolete]
    [HttpPost("session-close")]
    [ProducesResponseType(typeof(BetflagSessionCloseRequest.CloseSessionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Authenticate(
        BetflagSessionCloseRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}