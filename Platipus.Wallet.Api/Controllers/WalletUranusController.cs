namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.Uranus;
using Application.Requests.Wallets.Uranus.Base;
using Application.Requests.Wallets.Uranus.Data;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.NewFilterStyle;
using StartupSettings.Filters.Security.Uranus;

[Route("wallet/uranus/")]
[ServiceFilter(typeof(UranusMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(UranusSecurityFilter), Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.Uranus))]
[ProducesResponseType(typeof(UranusFailureResponse), StatusCodes.Status200OK)]
public sealed class WalletUranusController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletUranusController(IMediator mediator) => 
        _mediator = mediator;
    
    [HttpPost("balance")]
    [ProducesResponseType(typeof(UranusSuccessResponse<UranusBalanceData>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance(
        [FromBody] UranusBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("deposit")]
    [ProducesResponseType(typeof(UranusSuccessResponse<UranusCommonDataWithTransaction>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Deposit(
        [FromBody] UranusDepositRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("withdrawal")]
    [ProducesResponseType(typeof(UranusSuccessResponse<UranusCommonDataWithTransaction>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Withdraw(
        [FromBody] UranusWithdrawRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("rollback")]
    [ProducesResponseType(typeof(UranusSuccessResponse<UranusCommonDataWithTransaction>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Rollback(
        [FromBody] UranusRollbackRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("promoWin")]
    [ProducesResponseType(typeof(UranusSuccessResponse<UranusCommonDataWithTransaction>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PromoWin(
        [FromBody] UranusPromoWinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
}