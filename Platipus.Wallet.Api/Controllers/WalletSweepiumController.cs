using Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium;
using Platipus.Wallet.Api.Application.Responses.Sweepium;
using Platipus.Wallet.Api.Application.Responses.Sweepium.Base;
using Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle.Other;
using Platipus.Wallet.Api.StartupSettings.Filters.Security;

namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("wallet/sweepium/")]
[ServiceFilter(typeof(SweepiumMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(SweepiumSecurityFilter), Order = 2)]
[JsonSettingsName(WalletProvider.Sweepium)]
[ProducesResponseType(typeof(SweepiumErrorResponse), StatusCodes.Status200OK)]
public sealed class WalletSweepiumController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletSweepiumController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("UpdateBalance")]
    [ProducesResponseType(typeof(SweepiumStartUpdateBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateBalance(
        [FromBody] SweepiumStartUpdateBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("Bet")]
    [ProducesResponseType(typeof(SweepiumSuccessResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        [FromBody] SweepiumBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("Win")]
    [ProducesResponseType(typeof(SweepiumSuccessResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        [FromBody] SweepiumWinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("Rollback")]
    [ProducesResponseType(typeof(SweepiumSuccessResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Rollback(
        [FromBody] SweepiumRollbackRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("Start")]
    [ProducesResponseType(typeof(SweepiumStartUpdateBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Start(
        [FromBody] SweepiumStartUpdateBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}