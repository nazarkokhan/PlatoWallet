namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet.Base.Response;

using Results.ISoftBet;

public record SoftBetErrorResponse(
    string Code,
    string Message,
    string Action,
    bool Display) : SoftBetBaseResponse(SoftBetStatus.error);