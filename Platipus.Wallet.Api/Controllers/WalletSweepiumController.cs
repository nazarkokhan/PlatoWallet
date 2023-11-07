using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium;
using Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Base;
using Platipus.Wallet.Api.Application.Responses.Sweepium;
using Platipus.Wallet.Api.Application.Responses.Sweepium.Base;
using Platipus.Wallet.Api.Extensions.SecuritySign.Sweepium;
using Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;
using Platipus.Wallet.Api.StartupSettings.Filters.Security;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;

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
        [FromBody] SweepiumBoxRequest<SweepiumStartUpdateBalanceRequest> request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request.Data, cancellationToken)).ToActionResult();

    [HttpPost("Bet")]
    [ProducesResponseType(typeof(SweepiumSuccessResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        [FromBody] SweepiumBoxRequest<SweepiumBetRequest> request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request.Data, cancellationToken)).ToActionResult();

    [HttpPost("Win")]
    [ProducesResponseType(typeof(SweepiumSuccessResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        [FromBody] SweepiumBoxRequest<SweepiumWinRequest> request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request.Data, cancellationToken)).ToActionResult();

    [HttpPost("Rollback")]
    [ProducesResponseType(typeof(SweepiumSuccessResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Rollback(
        [FromBody] SweepiumBoxRequest<SweepiumRollbackRequest> request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request.Data, cancellationToken)).ToActionResult();
    
    [HttpPost("Start")]
    [ProducesResponseType(typeof(SweepiumStartUpdateBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Start(
        [FromBody] SweepiumBoxRequest<SweepiumStartUpdateBalanceRequest> request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request.Data, cancellationToken)).ToActionResult();
}

[Route("wallet/private/sweepium")]
[JsonSettingsName(WalletProvider.Sweepium)]
public class WalletSweepiumTestController : RestApiController
{
    [HttpPost("get-security-value")]
    public async Task<IActionResult> GetSecurityValue(
        string casinoId,
        string time,
        JsonDocument data,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var casino = await dbContext.Set<Casino>()
            .Where(c => c.Id == casinoId)
            .Select(
                c => new
                {
                    c.SignatureKey,
                    SecuritySignKey = c.SignatureKey
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return ResultFactory.Failure(ErrorCode.CasinoNotFound).ToActionResult();

        var dataString = data.RootElement.GetProperty("data").GetRawText();

        var hash = SweepiumSecurityHash.Compute(
            time,
            dataString,
            casino.SecuritySignKey);

        return Ok(hash);
    }
}