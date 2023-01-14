namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.PariMatch;
using Application.Requests.Wallets.PariMatch.Base;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.Filters;
//TODO add x-hub-consumer header
//TODO Add GetPariMatchLaunchUrl
[Route("wallet/parimatch")]
[MockedErrorActionFilter(Order = 1)]
[ProducesResponseType(typeof(PariMatchErrorResponse), StatusCodes.Status400BadRequest)]
public class WalletPariMatchController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletPariMatchController(IMediator mediator) => _mediator = mediator;

    [HttpPost("player-info")]
    [ProducesResponseType(typeof(PariMatchPlayerInfoRequest.PariMatchPlayerInfoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance(
        PariMatchPlayerInfoRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(PariMatchBetRequest.PariMatchBetResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        PariMatchBetRequest request,
        CancellationToken cancellationToken)
        => ((Ok(await _mediator.Send(request, cancellationToken))));

    [HttpPost("win")]
    [ProducesResponseType(typeof(PariMatchWinRequest.PariMatchWinResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        PariMatchWinRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));

    [HttpPost("cancel")]
    [ProducesResponseType(typeof(PariMatchCancelRequest.PariMatchCancelResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(
        PariMatchCancelRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));

    
}