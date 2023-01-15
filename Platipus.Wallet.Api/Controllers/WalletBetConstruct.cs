namespace Platipus.Wallet.Api.Controllers;
// ReSharper disable UnusedParameter.Global

using Abstract;
using Application.Requests.Wallets.BetConstruct;
using Application.Requests.Wallets.BetConstruct.Base;
using Application.Requests.Wallets.Betflag;
using Application.Requests.Wallets.Betflag.Base;
using Application.Results.BetConstruct;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;

[Route("wallet/betflag")]
[MockedErrorActionFilter(Order = 1)]
[BetConstructVerifyHashFilterAttribute(Order = 2)]
[ProducesResponseType(typeof(BetConstructErrorResponse), StatusCodes.Status400BadRequest)]
public class WalletBetConstructController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletBetConstructController(IMediator mediator) => _mediator = mediator;

    [HttpPost("player-info")]
    [ProducesResponseType(typeof(BetConstructPlayerInfoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance(
        BetConstructGetPlayerInfoRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));

    [HttpPost("withdraw")]
    [ProducesResponseType(typeof(BetConstructBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        BetConstructWithdrawRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));

    [HttpPost("deposit")]
    [ProducesResponseType(typeof(BetConstructBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        BetConstructDepositRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));

    [HttpPost("rollback")]
    [ProducesResponseType(typeof(BetConstructBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(
        BetConstructRollbackTransactionRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));


}