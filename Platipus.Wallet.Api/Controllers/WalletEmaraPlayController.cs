namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.EmaraPlay;
using Application.Requests.Wallets.EmaraPlay.Base;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.Filters;

[Microsoft.AspNetCore.Components.Route("wallet/emaraplay")]
[MockedErrorActionFilter(Order = 1)]
[EmaraPlayVerifyHashFilter(Order = 2)]
[ProducesResponseType(typeof(EmaraPlayBaseResponse), StatusCodes.Status400BadRequest)]
public class WalletEmaraPlayController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletEmaraPlayController(IMediator mediator) => _mediator = mediator;

    [HttpPost("balance")]
    [ProducesResponseType(typeof(EmaraPlayBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        [FromHeader(Name = "Authorization")] string hash,
        EmaraPlayBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(EmaraPlayBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        [FromHeader(Name = "Authorization")] string hash,
        EmaraPlayBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("win")]
    [ProducesResponseType(typeof(EmaraPlayBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Result(
        [FromHeader(Name = "Authorization")] string hash,
        EmaraPlayResultRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("rollback")]
    [ProducesResponseType(typeof(EmaraPlayBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Refund(
        [FromHeader(Name = "Authorization")] string hash,
        EmaraPlayRefundRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}