namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.Wallets.Everymatrix;
using Application.Requests.Wallets.Psw;
using Application.Requests.Wallets.Psw.Base.Response;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.Filters;

[Route("wallet/psw")]
[MockedErrorActionFilter(Order = 1)]
[EveryMatrixHashFilter(Order = 2)]
[ProducesResponseType(typeof(PswErrorResponse), StatusCodes.Status400BadRequest)]
public class WalletPswController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletPswController(IMediator mediator) => _mediator = mediator;

    [HttpPost("balance")]
    [ProducesResponseType(typeof(PswBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance(
        [FromHeader(Name = PswHeaders.XRequestSign)] string sign,
        EverymatrixGetBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(PswBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        [FromHeader(Name = PswHeaders.XRequestSign)] string sign,
        PswBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("win")]
    [ProducesResponseType(typeof(PswBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        [FromHeader(Name = PswHeaders.XRequestSign)] string sign,
        PswWinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("rollback")]
    [ProducesResponseType(typeof(PswBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Rollback(
        [FromHeader(Name = PswHeaders.XRequestSign)] string sign,
        PswRollbackRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("award")]
    [ProducesResponseType(typeof(PswBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Award(
        [FromHeader(Name = PswHeaders.XRequestSign)] string sign,
        PswAwardRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}

