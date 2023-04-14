namespace Platipus.Wallet.Api.Controllers;

using System.Net.Mime;
using Abstract;
using Application.Extensions;
using Application.Requests.Wallets.Sw;
using Application.Requests.Wallets.Sw.Base.Response;
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

[Route("wallet/sw")]
[MockedErrorActionFilter(Order = 1)]
[SwSecurityFilter(Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.Sw))]
[ProducesResponseType(typeof(SwErrorResponse), StatusCodes.Status200OK)]
[Consumes("application/x-www-form-urlencoded")]
[Produces(MediaTypeNames.Application.Json)]
public class WalletSwController : ApiController
{
    private readonly IMediator _mediator;

    public WalletSwController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("balance-md5")]
    [ProducesResponseType(typeof(SwBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> BalanceMd5(
        [FromForm] SwGetBalanceMd5Request request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("balance-hash")]
    [ProducesResponseType(typeof(SwBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> BalanceHash(
        [FromForm] SwGetBalanceHashRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet-win")]
    [ProducesResponseType(typeof(SwBetWinRefundFreespinResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> BetWin(
        [FromForm] SwBetWinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("username")]
    [ProducesResponseType(typeof(SwGetUserNameRequest.SwUserNameResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Username(
        [FromForm] SwGetUserNameRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("refund")]
    [ProducesResponseType(typeof(SwBetWinRefundFreespinResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Refund(
        [FromForm] SwRefundRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("freespin")]
    [ProducesResponseType(typeof(SwBetWinRefundFreespinResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Freespin(
        [FromForm] SwFreespinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}

[Route("wallet/private/softswiss")]
[JsonSettingsName(nameof(CasinoProvider.Sw))]
public class WalletSwPrivateController : RestApiController
{
    [HttpPost("get-security-value/hash")]
    public async Task<IActionResult> GetSecurityValueHash(
        string userName,
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

        var securityValue = user.Map(u => SwSecurityHash.Compute(u.Casino.ProviderId, u.UserId, u.Casino.SignatureKey));

        return Ok(securityValue);
    }

    [HttpPost("get-security-value/md5")]
    public async Task<IActionResult> GetSecurityValueMd5(
        string userName,
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

        var securityValue = user.Map(u => SwSecurityMd5.Compute(u.Casino.ProviderId, u.UserId, u.Casino.SignatureKey));

        return Ok(securityValue);
    }
}