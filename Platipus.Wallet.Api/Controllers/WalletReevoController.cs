namespace Platipus.Wallet.Api.Controllers;

using System.Web;
using Abstract;
using Application.Extensions;
using Application.Requests.Wallets.Reevo;
using Application.Requests.Wallets.Reevo.Base;
using Application.Results.Reevo;
using Application.Results.Reevo.WithData;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;
using StartupSettings.Filters.Security;

[Route("wallet/reevo")]
[MockedErrorActionFilter(Order = 1)]
[ReevoSecurityFilter(Order = 2)]
[JsonSettingsName(CasinoProvider.Reevo)]
[ProducesResponseType(typeof(ReevoErrorResponse), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ReevoSuccessResponse), StatusCodes.Status200OK)]
public class WalletReevoController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletReevoController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> SingleEndpoint(
        [FromQuery] ReevoSingleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            IRequest<IReevoResult<ReevoSuccessResponse>>? concreteRequest = request.Action switch
            {
                "balance" => request.Map(
                    r => new ReevoBalanceRequest(
                        r.CallerId,
                        r.CallerPassword,
                        r.Action,
                        r.RemoteId,
                        r.Username,
                        r.GameIdHash ?? throw new ReevoMissingParameterException(nameof(r.GameIdHash)),
                        r.SessionId,
                        r.GameSessionId,
                        r.Key)),
                "debit" => request.Map(
                    r => new ReevoDebitRequest(
                        r.CallerId,
                        r.CallerPassword,
                        r.Action,
                        r.RemoteId,
                        r.Username,
                        r.SessionId,
                        r.Amount ?? throw new ReevoMissingParameterException(nameof(r.Amount)),
                        r.GameIdHash ?? throw new ReevoMissingParameterException(nameof(r.GameIdHash)),
                        r.TransactionId ?? throw new ReevoMissingParameterException(nameof(r.TransactionId)),
                        r.RoundId ?? throw new ReevoMissingParameterException(nameof(r.RoundId)),
                        r.GameplayFinal,
                        r.IsFreeRoundBet ?? throw new ReevoMissingParameterException(nameof(r.IsFreeRoundBet)),
                        r.FreeRoundId,
                        r.Fee,
                        r.JackpotContributionInAmount,
                        r.GameSessionId,
                        r.Key)),
                "credit" => request.Map(
                    r => new ReevoCreditRequest(
                        r.CallerId,
                        r.CallerPassword,
                        r.Action,
                        r.RemoteId,
                        r.Username,
                        r.SessionId,
                        r.Amount ?? throw new ReevoMissingParameterException(nameof(r.Amount)),
                        r.GameIdHash ?? throw new ReevoMissingParameterException(nameof(r.GameIdHash)),
                        r.TransactionId ?? throw new ReevoMissingParameterException(nameof(r.TransactionId)),
                        r.RoundId ?? throw new ReevoMissingParameterException(nameof(r.RoundId)),
                        r.GameplayFinal,
                        r.IsFreeRoundWin ?? throw new ReevoMissingParameterException(nameof(r.IsFreeRoundWin)),
                        r.FreeroundSpinsRemaining,
                        r.FreeroundCompleted,
                        r.FreeRoundId,
                        r.IsJackpotWin ?? throw new ReevoMissingParameterException(nameof(r.IsJackpotWin)),
                        r.JackpotWinInAmount,
                        r.GameSessionId,
                        r.Key)),
                "rollback" => request.Map(
                    r => new ReevoRollbackRequest(
                        r.CallerId,
                        r.CallerPassword,
                        r.Action,
                        r.RemoteId,
                        r.Username,
                        r.SessionId,
                        r.Amount ?? throw new ReevoMissingParameterException(nameof(r.Amount)),
                        r.GameIdHash,
                        r.TransactionId ?? throw new ReevoMissingParameterException(nameof(r.TransactionId)),
                        r.RoundId ?? throw new ReevoMissingParameterException(nameof(r.RoundId)),
                        r.GameplayFinal,
                        r.GameSessionId,
                        r.Key)),
                _ => null
            };
            if (concreteRequest is null)
                return ReevoResultFactory
                    .Failure(request.Action is "debit" ? ReevoErrorCode.BetRefused : ReevoErrorCode.InternalError)
                    .ToActionResult();

            var result = await _mediator.Send(concreteRequest, cancellationToken);

            return result.ToActionResult();
        }
        catch (ReevoMissingParameterException e)
        {
            return ReevoResultFactory.Failure(
                    request.Action is "debit" ? ReevoErrorCode.BetRefused : ReevoErrorCode.InternalError,
                    e)
                .ToActionResult();
        }
        catch (Exception e)
        {
            return ReevoResultFactory.Failure(
                    request.Action is "debit" ? ReevoErrorCode.BetRefused : ReevoErrorCode.InternalError,
                    e)
                .ToActionResult();
        }
    }

    [HttpPost("private/test/get-security-value")]
    public async Task<IActionResult> GetSecurityValue(
        string userName,
        string url,
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
                        c.Casino.SignatureKey,
                    }
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return ResultFactory.Failure(ErrorCode.UserNotFound).ToActionResult();

        var requestQueryString2 = new Uri(url).Query;
        var nameValueCollection = HttpUtility.ParseQueryString(requestQueryString2);
        var requestQueryString = nameValueCollection.Keys.Cast<string?>()
            .ToDictionary(o => o!, o => new StringValues(nameValueCollection.GetValues(o)));
        requestQueryString.Remove("key");
        var withoutKey = QueryString.Create(requestQueryString).ToString();
        var securityValue = ReevoSecurityHash.Compute(withoutKey, user.Casino.SignatureKey);

        return Ok(securityValue);
    }
}