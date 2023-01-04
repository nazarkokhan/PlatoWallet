namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.Wallets.Everymatrix;
using Application.Requests.Wallets.Everymatrix.Base.Response;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.Filters;

[Route("wallet/everymatrix")]
[MockedErrorActionFilter(Order = 1)]
[EveryMatrixVerifySignatureFilter(Order = 2)]
[ProducesResponseType(typeof(EverymatrixErrorResponse), StatusCodes.Status400BadRequest)]
public class WalletEveryMatrixController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletEveryMatrixController(IMediator mediator) => _mediator = mediator;

    [HttpPost("get-balance")]
    [ProducesResponseType(typeof(EveryMatrixBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance(
        EverymatrixGetBalanceRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));

    [HttpPost("bet")]
    [ProducesResponseType(typeof(EveryMatrixBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        EverymatrixBetRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));

    [HttpPost("win")]
    [ProducesResponseType(typeof(EveryMatrixBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        EverymatrixWinRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));

    [HttpPost("cancel")]
    [ProducesResponseType(typeof(EveryMatrixBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(
        EverymatrixCancelRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));

    [HttpPost("reconciliation")]
    [ProducesResponseType(typeof(EveryMatrixBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Reconciliation(
        EveryMatrixReconciliationRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));

    [HttpPost("authenticate")]
    [ProducesResponseType(typeof(EverymatrixRequestAuthenticateRequest.EveryMatrixAuthenticationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Authenticate(
        EverymatrixRequestAuthenticateRequest request,
        CancellationToken cancellationToken)
        => (Ok(await _mediator.Send(request, cancellationToken)));
}

