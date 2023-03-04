namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.BetConstruct;
using Application.Requests.Wallets.BetConstruct.Base.Response;
using Domain.Entities;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartupSettings.Filters;
using StartupSettings.Filters.TODO;

[Route("wallet/betconstruct")]
[MockedErrorActionFilter(Order = 1)]
[BetConstructVerifyHashFilter (Order = 2)]
// [JsonSettingsName(nameof(CasinoProvider.Everymatrix))]
// [ProducesResponseType(typeof(EverymatrixErrorResponse), StatusCodes.Status200OK)]
public class WalletBetConstructController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletBetConstructController(IMediator mediator) => _mediator = mediator;


    [HttpPost("GetPlayerInfo")]
    [ProducesResponseType(typeof(BetConstructGetPlayerInfoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance(
        BetConstructGetPlayerInfoRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("Withdraw")]
    [ProducesResponseType(typeof(BetConstructBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        BetConstructWithdrawRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("Deposit")]
    [ProducesResponseType(typeof(BetConstructBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        BetConstructDepositRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("Rollback")]
    [ProducesResponseType(typeof(BetConstructBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(
        BetConstructRollbackTransactionRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();


    [HttpPost("private/test/get-security-value")]
    public async Task<IActionResult> GetSecurityValue(
        string username,
        string route,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Set<User>()
            .Where(u => u.Username == username)
            .Select(u => new { CasinoSignatureKey = u.Casino.SignatureKey })
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (user is null)
            return ResultFactory.Failure(ErrorCode.UserNotFound).ToActionResult();

        var securityValue = EverymatrixSecurityHash.Compute(route, user.CasinoSignatureKey);

        return Ok(securityValue);
    }
}