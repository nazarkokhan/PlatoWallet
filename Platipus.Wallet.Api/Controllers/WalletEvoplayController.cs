namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.Evoplay;
using Application.Requests.Wallets.Evoplay.Base;
using Application.Requests.Wallets.Evoplay.Data;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.NewFilterStyle;
using StartupSettings.Filters.Security.Evoplay;

[Route("wallet/evoplay/")]
[ServiceFilter(typeof(EvoplayMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(EvoplaySecurityFilter), Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.Evoplay))]
[ProducesResponseType(typeof(EvoplayFailureResponse), StatusCodes.Status200OK)]
public sealed class WalletEvoplayController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletEvoplayController(IMediator mediator) => 
        _mediator = mediator;
    
    [HttpPost("balance")]
    [ProducesResponseType(typeof(EvoplaySuccessResponse<EvoplayBalanceData>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance(
        [FromBody] EvoplayBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("deposit")]
    [ProducesResponseType(typeof(EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Deposit(
        [FromBody] EvoplayDepositRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("withdrawal")]
    [ProducesResponseType(typeof(EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Withdraw(
        [FromBody] EvoplayWithdrawRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("rollback")]
    [ProducesResponseType(typeof(EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Rollback(
        [FromBody] EvoplayRollbackRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("promoWin")]
    [ProducesResponseType(typeof(EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Award(
        [FromBody] EvoplayAwardRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
}