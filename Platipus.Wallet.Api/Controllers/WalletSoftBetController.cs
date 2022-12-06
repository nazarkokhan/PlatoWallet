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

[Route("wallet/softbet")]
[MockedErrorActionFilter(Order = 1)]
[JsonSettingsName(nameof(CasinoProvider.SoftBet))]
[ProducesResponseType(typeof(SoftBetErrorResponse), StatusCodes.Status400BadRequest)]
public class WalletSoftBetController : RestApiController
{
    private readonly IMediator _mediator;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly WalletDbContext _context;

    public WalletSoftBetController(IMediator mediator, IOptionsMonitor<JsonOptions> options, WalletDbContext context)
    {
        _mediator = mediator;
        _jsonSerializerOptions = options.Get(CasinoProvider.SoftBet.ToString()).JsonSerializerOptions;
        _context = context;
    }

    [HttpPost("service/{providerId}")]
    [ProducesResponseType(typeof(SoftBetBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        string providerId,
        string hash,
        SoftBetSingleRequest request,
        CancellationToken cancellationToken)
    {
        var casino = await _context.Set<Casino>()
            .Where(c => c.Users.Select(u => u.UserName).Contains(request.Username))
            .Select(c => new {c.SignatureKey})
            .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return SoftBetResultFactory.Failure(SoftBetError.IncorrectFormatOfParameters).ToActionResult();

        var rawRequestBytes = (byte[])HttpContext.Items["rawRequestBytes"]!;
        var isValidHash = SoftBetRequestHash.IsValidSign(hash, rawRequestBytes, casino.SignatureKey);
        if (!isValidHash)
            return SoftBetResultFactory.Failure(SoftBetError.PlayerAuthenticationFailed).ToActionResult();

        var action = request.Action;
        var actionParams = request.Action.Parameters;

        var validToken = action.Command is "initsession" ? request.Token : request.SessionId;
        var session = await _context.Set<Session>()
            .Where(c => c.Id == validToken)
            .Select(c => new {c.ExpirationDate})
            .FirstOrDefaultAsync(cancellationToken);

        if (session is null || session.ExpirationDate < DateTime.UtcNow)
            return SoftBetResultFactory.Failure(SoftBetError.PlayerAuthenticationFailed).ToActionResult();

        object? payloadRequestObj = null;
        switch (action.Command)
        {
            case "initsession":
                payloadRequestObj = new SoftBetInitSessionRequest(request.Token, request.Username);
                break;
            case "balance":
                payloadRequestObj = new SoftBetGetBalanceRequest(request.SessionId, request.Username);
                break;
            case "bet":
                var betParams = actionParams.Deserialize<BetParameters>(_jsonSerializerOptions)!;
                payloadRequestObj = new SoftBetBetRequest(
                    request.SessionId,
                    request.Username,
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
                    request.Username,
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
                    request.Username,
                    request.ProviderGameId,
                    cancelParams.RoundId,
                    cancelParams.TransactionId);
                break;
            case "end":
                var endParams = actionParams.Deserialize<EndParameters>(_jsonSerializerOptions)!;
                payloadRequestObj = new SoftBetEndRequest(
                    request.SessionId,
                    request.Username,
                    endParams.SessionStatus);
                break;
        }

        if (payloadRequestObj is null)
            return SoftBetResultFactory.Failure(SoftBetError.IncorrectFormatOfParameters).ToActionResult();

        var responseObj = await _mediator.Send(payloadRequestObj, cancellationToken);
        if (responseObj is not ISoftBetResult response)
            return SoftBetResultFactory.Failure(SoftBetError.GeneralRequestError).ToActionResult();

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