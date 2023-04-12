namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.PariMatch;
using Application.Requests.Wallets.PariMatch.Base;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;

[Route("wallet/parimatch")]
[MockedErrorActionFilter(Order = 1)]
[JsonSettingsName(nameof(CasinoProvider.PariMatch))]
[ProducesResponseType(typeof(ParimatchErrorResponse), StatusCodes.Status400BadRequest)]
public class WalletPariMatchController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletPariMatchController(IMediator mediator) => _mediator = mediator;

    [HttpPost("player-info")]
    [ProducesResponseType(typeof(ParimatchPlayerInfoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance(
        ParimatchPlayerInfoBaseRequest baseRequest,
        CancellationToken cancellationToken)
        => (await _mediator.Send(baseRequest, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(ParimatchBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        ParimatchBetBaseRequest baseRequest,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(baseRequest, cancellationToken));

    [HttpPost("win")]
    [ProducesResponseType(typeof(ParimatchBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        ParimatchWinBaseRequest baseRequest,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(baseRequest, cancellationToken)));

    [HttpPost("cancel")]
    [ProducesResponseType(typeof(ParimatchBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(
        ParimatchCancelBaseRequest baseRequest,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(baseRequest, cancellationToken)));
}

