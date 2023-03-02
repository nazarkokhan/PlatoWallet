namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet.Base;

using Requests.Base;

public record SoftBetSingleRequest(
    string ProviderGameId,
    int LicenseeId,
    string Token,
    string SessionId,
    string PlayerId,
    string Username,
    string Currency,
    int IsbSkinId,
    int IsbGameId,
    string State,
    string Operator,
    SoftBetAction Action) : IBaseWalletRequest;