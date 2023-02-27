namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Extensions;
using Application.Requests.Wallets.Reevo;
using Application.Requests.Wallets.Reevo.Base;
using Application.Results.Reevo;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;
using StartupSettings.Filters.Security;

[Route("wallet/reevo")]
[MockedErrorActionFilter(Order = 1)]
[ReevoSecurityFilter(Order = 0)]
[JsonSettingsName(nameof(CasinoProvider.Reevo))]
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
            object? concreteRequest = request.Action switch
            {
                "balance" => request.Map(
                    r => new ReevoBalanceRequest(
                        r.CallerId,
                        r.CallerPassword,
                        r.Action,
                        r.RemoteId ?? throw new ReevoMissingParameterException(nameof(r.RemoteId)),
                        r.Username,
                        r.GameIdHash  ?? throw new ReevoMissingParameterException(nameof(r.GameIdHash)),
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
                        r.GameIdHash  ?? throw new ReevoMissingParameterException(nameof(r.GameIdHash)),
                        r.TransactionId ?? throw new ReevoMissingParameterException(nameof(r.TransactionId)),
                        r.RoundId ?? throw new ReevoMissingParameterException(nameof(r.RoundId)),
                        r.GameplayFinal,
                        r.GameSessionId,
                        r.Key)),
                _ => null
            };
            if (concreteRequest is null)
                return ReevoResultFactory
                    .Failure(request.Action is "debit" ? ReevoErrorCode.BetRefused : ReevoErrorCode.GeneralError)
                    .ToActionResult();

            var result = await _mediator.Send(concreteRequest, cancellationToken);

            if (result is not IReevoResult reevoResult)
                return ReevoResultFactory
                    .Failure(request.Action is "debit" ? ReevoErrorCode.BetRefused : ReevoErrorCode.GeneralError)
                    .ToActionResult();

            return reevoResult.ToActionResult();
        }
        catch (ReevoMissingParameterException e)
        {
            return ReevoResultFactory.Failure(
                    request.Action is "debit" ? ReevoErrorCode.BetRefused : ReevoErrorCode.GeneralError,
                    e)
                .ToActionResult();
        }
        catch (Exception e)
        {
            return ReevoResultFactory.Failure(
                    request.Action is "debit" ? ReevoErrorCode.BetRefused : ReevoErrorCode.GeneralError,
                    e)
                .ToActionResult();
        }
    }

    [Obsolete]
    [HttpGet("balance")]
    public async Task<IActionResult> Balance(
        [FromQuery] ReevoBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [Obsolete]
    [HttpGet("debit")]
    public async Task<IActionResult> Bet(
        [FromQuery] ReevoDebitRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [Obsolete]
    [HttpGet("credit")]
    public async Task<IActionResult> Win(
        [FromQuery] ReevoCreditRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [Obsolete]
    [HttpGet("rollback")]
    public async Task<IActionResult> Rollback(
        [FromQuery] ReevoRollbackRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}