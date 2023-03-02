namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.Betflag;
using Application.Requests.Wallets.Betflag.Base;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;
using StartupSettings.Filters.Security;

[Route("wallet/betflag")]
[MockedErrorActionFilter(Order = 1)]
[BetflagSecurityFilter(Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.Betflag))]
[ProducesResponseType(typeof(BetflagErrorResponse), StatusCodes.Status200OK)]
public class WalletBetflagController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletBetflagController(IMediator mediator) => _mediator = mediator;

    [HttpPost("balance")]
    [ProducesResponseType(typeof(BetflagBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        BetflagBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(BetflagBetWinCancelResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        BetflagBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("win")]
    [ProducesResponseType(typeof(BetflagBetWinCancelResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        BetflagWinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("cancel")]
    [ProducesResponseType(typeof(BetflagBetWinCancelResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(
        BetflagCancelRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("authenticate")]
    [ProducesResponseType(typeof(BetflagBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Authenticate(
        BetflagAuthenticateRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [Obsolete]
    [HttpPost("session-close")]
    [ProducesResponseType(typeof(BetflagSessionCloseRequest.CloseSessionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> SessionClose(
        BetflagSessionCloseRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("private/test/get-security-value")]
    public async Task<IActionResult> GetSecurityValue(
        string userName,
        long timestamp,
        string key,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Set<User>()
            .Where(c => c.Username == userName)
            .Select(
                c => new
                {
                    UserId = c.Id,
                    Casino = new
                    {
                        ProviderId = c.Casino.InternalId,
                        c.Casino.SignatureKey,
                    }
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return ResultFactory.Failure(ErrorCode.UserNotFound).ToActionResult();

        var securityValue = BetflagSecurityHash.Compute(key, timestamp, user.Casino.SignatureKey);

        return Ok(securityValue);
    }
}