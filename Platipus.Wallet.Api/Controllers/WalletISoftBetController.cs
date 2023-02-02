namespace Platipus.Wallet.Api.Controllers;

using System.Text.Json;
using Abstract;
using Application.Requests.Wallets.SoftBet;
using Application.Requests.Wallets.SoftBet.Base;
using Application.Requests.Wallets.SoftBet.Base.Response;
using Application.Results.ISoftBet;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;

[Route("wallet/isoftbet")]
[MockedErrorActionFilter(Order = 1)]
[JsonSettingsName(nameof(CasinoProvider.SoftBet))]
[ProducesResponseType(typeof(SoftBetErrorResponse), StatusCodes.Status400BadRequest)]
public class WalletISoftBetController : RestApiController
{
    private readonly IMediator _mediator;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly WalletDbContext _context;

    public WalletISoftBetController(IMediator mediator, IOptionsMonitor<JsonOptions> options, WalletDbContext context)
    {
        _mediator = mediator;
        _jsonSerializerOptions = options.Get(CasinoProvider.SoftBet.ToString()).JsonSerializerOptions;
        _context = context;
    }

    [HttpPost("service/{providerId}")]
    [ProducesResponseType(typeof(SoftBetBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        int providerId,
        string hash,
        SoftBetSingleRequest request,
        CancellationToken cancellationToken)
    {
        var casino = await _context.Set<Casino>()
            .Where(
                c =>
                    c.SwProviderId == request.LicenseeId
                 && c.Users.Select(u => u.UserName).Contains(request.Username))
            .Select(c => new { c.SignatureKey })
            .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return SoftBetResultFactory.Failure(SoftBetErrorMessage.IncorrectFormatOfParameters).ToActionResult();

        var rawRequestBytes = (byte[])HttpContext.Items["rawRequestBytes"]!;
        var isValidHash = SoftbetSecurityHash.IsValid(hash, rawRequestBytes, casino.SignatureKey);
        if (!isValidHash)
            return SoftBetResultFactory.Failure(SoftBetErrorMessage.PlayerAuthenticationFailed).ToActionResult();

        var action = request.Action;
        var actionParams = request.Action.Parameters;

        if (action.Command is "initsession")
        {
            var initSessionRequest = new SoftBetInitSessionRequest(request.Token, request.Username);
            var initSessionResponse = await _mediator.Send(initSessionRequest, cancellationToken);
            return initSessionResponse.ToActionResult();
        }

        var session = await _context.Set<Session>()
            .Where(c => c.Id == request.SessionId)
            .Select(
                c => new
                {
                    c.Id,
                    c.ExpirationDate
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (session is null || session.ExpirationDate < DateTime.UtcNow)
            return SoftBetResultFactory.Failure(SoftBetErrorMessage.PlayerAuthenticationFailed).ToActionResult();

        object? payloadRequestObj = null;
        switch (action.Command)
        {
            case "balance":
                payloadRequestObj = new SoftBetGetBalanceRequest(request.SessionId, request.Username);
                break;
            case "bet":
                var betParams = actionParams.Deserialize<BetParameters>(_jsonSerializerOptions)!;
                payloadRequestObj = new SoftBetBetRequest(
                    request.SessionId,
                    request.PlayerId,
                    request.Currency,
                    request.ProviderGameId,
                    betParams.Amount,
                    betParams.RoundId,
                    betParams.TransactionId);
                break;
            case "win":
                var winParams = actionParams.Deserialize<WinParameters>(_jsonSerializerOptions)!;
                payloadRequestObj = new SoftBetWinRequest(
                    request.SessionId,
                    request.PlayerId,
                    request.Currency,
                    request.ProviderGameId,
                    winParams.Amount,
                    winParams.RoundId,
                    winParams.TransactionId,
                    winParams.CloseRound);
                break;
            case "cancel":
                var cancelParams = actionParams.Deserialize<CancelParameters>(_jsonSerializerOptions)!;
                payloadRequestObj = new SoftBetCancelRequest(
                    request.SessionId,
                    request.PlayerId,
                    request.ProviderGameId,
                    cancelParams.RoundId,
                    cancelParams.TransactionId);
                break;
            case "end":
                var endParams = actionParams.Deserialize<EndParameters>(_jsonSerializerOptions)!;
                payloadRequestObj = new SoftBetEndRequest(
                    request.SessionId,
                    request.PlayerId,
                    endParams.SessionStatus);
                break;
        }

        if (payloadRequestObj is null)
            return SoftBetResultFactory.Failure(SoftBetErrorMessage.IncorrectFormatOfParameters).ToActionResult();

        var responseObj = await _mediator.Send(payloadRequestObj, cancellationToken);
        if (responseObj is not ISoftBetResult response)
            return SoftBetResultFactory.Failure(SoftBetErrorMessage.GeneralRequestError).ToActionResult();

        return response.ToActionResult();
    }
}

public record BetParameters(
    int Amount,
    string RoundId,
    string TransactionId);

public record WinParameters(
    int Amount,
    string RoundId,
    string TransactionId,
    bool CloseRound);

public record CancelParameters(
    int Amount,
    string RoundId,
    string TransactionId);

public record EndParameters(string SessionStatus);