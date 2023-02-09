namespace Platipus.Wallet.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;
using Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix.Base.Response;
using Platipus.Wallet.Api.Controllers.Abstract;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Api.StartupSettings.ControllerSpecificJsonOptions;
using Platipus.Wallet.Api.StartupSettings.Filters;
using Platipus.Wallet.Api.StartupSettings.Filters.Security;
using Platipus.Wallet.Domain.Entities.Enums;

[Route("wallet/everymatrix")]
[MockedErrorActionFilter(Order = 1)]
[EverymatrixSecurityFilter(Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.Everymatrix))]
[ProducesResponseType(typeof(EverymatrixErrorResponse), StatusCodes.Status200OK)]
public class WalletEverymatrixController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletEverymatrixController(IMediator mediator) => _mediator = mediator;

    [HttpPost("Authenticate")]
    [ProducesResponseType(typeof(EverymatrixAuthenticateRequest.EverymatrixAuthenticationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Authenticate(
        EverymatrixAuthenticateRequest authenticateRequest,
        CancellationToken cancellationToken)
        => (await _mediator.Send(authenticateRequest, cancellationToken)).ToActionResult();

    [HttpPost("GetBalance")]
    [ProducesResponseType(typeof(EverymatrixBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance(
        EverymatrixGetBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("Bet")]
    [ProducesResponseType(typeof(EverymatrixBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        EverymatrixBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("Win")]
    [ProducesResponseType(typeof(EverymatrixBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        EverymatrixWinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("Cancel")]
    [ProducesResponseType(typeof(EverymatrixBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(
        EverymatrixCancelRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("Reconciliation")]
    [ProducesResponseType(typeof(EverymatrixReconciliationRequest.ReconciliationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Reconciliation(
        EverymatrixReconciliationRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}


