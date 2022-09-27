namespace PlatipusWallet.Api.Controllers;

using Abstract;
using Application.Requests.Base.Responses;
using Application.Requests.Wallet;
using Extensions;
using Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("wallet")]
// [Route("")]
[ErrorMockActionFilter]
public class WalletController : ApiController
{
    private readonly IMediator _mediator;

    public WalletController(IMediator mediator) => _mediator = mediator;

    [HttpPost("balance")]
    [ProducesResponseType(typeof(BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        [FromHeader(Name = "X-REQUEST-SIGN")] string sign,
        GetBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        [FromHeader(Name = "X-REQUEST-SIGN")] string sign,
        BetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("win")]
    [ProducesResponseType(typeof(BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        [FromHeader(Name = "X-REQUEST-SIGN")] string sign,
        WinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("rollback")]
    [ProducesResponseType(typeof(BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Rollback(
        [FromHeader(Name = "X-REQUEST-SIGN")] string sign,
        RollbackRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("award")]
    [ProducesResponseType(typeof(BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Award(
        [FromHeader(Name = "X-REQUEST-SIGN")] string sign,
        AwardRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}