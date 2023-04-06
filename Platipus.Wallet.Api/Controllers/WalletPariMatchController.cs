namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.PariMatch;
using Application.Requests.Wallets.TODO.PariMatch.Base;
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
        ParimatchPlayerInfoRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(ParimatchBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        ParimatchBetRequest request,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request, cancellationToken));

    [HttpPost("win")]
    [ProducesResponseType(typeof(ParimatchBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        ParimatchWinRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));

    [HttpPost("cancel")]
    [ProducesResponseType(typeof(ParimatchBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(
        ParimatchCancelRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));
}

