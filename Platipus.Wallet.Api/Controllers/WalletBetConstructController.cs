namespace Platipus.Wallet.Api.Controllers;

using System.Text.Json;
using Abstract;
using Application.Requests.Wallets.BetConstruct;
using Application.Requests.Wallets.BetConstruct.Base;
using Application.Requests.Wallets.BetConstruct.Base.Response;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.NewFilterStyle;
using StartupSettings.Filters.Security;

[Route("wallet/betconstruct")]
[ServiceFilter(typeof(BetconstructMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(BetconstructSecurityFilter), Order = 2)]
[JsonSettingsName(CasinoProvider.BetConstruct)]
[ProducesResponseType(typeof(BetconstructErrorResponse), StatusCodes.Status200OK)]
public class WalletBetConstructController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletBetConstructController(IMediator mediator) => _mediator = mediator;

    [HttpPost("GetPlayerInfo")]
    [ProducesResponseType(typeof(BetconstructGetPlayerInfoRequest.GetPlayerInfoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlayerInfo(
        BetconstructBoxRequest<BetconstructGetPlayerInfoRequest> request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request.Data, cancellationToken)).ToActionResult();

    [HttpPost("Withdraw")]
    [ProducesResponseType(typeof(BetconstructPlayResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Withdraw(
        BetconstructBoxRequest<BetConstructWithdrawRequest> request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request.Data, cancellationToken)).ToActionResult();

    [HttpPost("Deposit")]
    [ProducesResponseType(typeof(BetconstructPlayResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Deposit(
        BetconstructBoxRequest<BetconstructDepositRequest> request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request.Data, cancellationToken)).ToActionResult();

    [HttpPost("RollbackTransaction")]
    [ProducesResponseType(typeof(BetconstructPlayResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Rollback(
        BetconstructBoxRequest<BetConstructRollbackTransactionRequest> request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request.Data, cancellationToken)).ToActionResult();
}

[Route("wallet/private/betconstruct")]
[JsonSettingsName(CasinoProvider.BetConstruct)]
public class WalletBetConstructTestController : RestApiController
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

        var hash = BetConstructSecurityHash.Compute(
            time,
            dataString,
            casino.SecuritySignKey);

        return Ok(hash);
    }
}