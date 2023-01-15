namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.Betflag;
using Application.Requests.Wallets.Betflag.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;

[Route("wallet/betflag")]
// [MockedErrorActionFilter(Order = 1)]
[BetflagVerifyHashFilter(Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.Betflag))]
[ProducesResponseType(typeof(BetflagErrorResponse), StatusCodes.Status400BadRequest)]
public class WalletBetflagController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletBetflagController(IMediator mediator) => _mediator = mediator;

    [HttpPost("get-balance")]
    [ProducesResponseType(typeof(BetflagBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance(
        BetflagBalanceRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));

    [HttpPost("bet")]
    [ProducesResponseType(typeof(BetflagBetWinCancelResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        BetflagBetRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));

    [HttpPost("win")]
    [ProducesResponseType(typeof(BetflagBetWinCancelResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        BetflagWinRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));

    [HttpPost("cancel")]
    [ProducesResponseType(typeof(BetflagBetWinCancelResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(
        BetflagCancelRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));


    [HttpPost("authenticate")]
    [ProducesResponseType(typeof(BetflagBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Authenticate(
        BetflagAuthenticateRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));
}