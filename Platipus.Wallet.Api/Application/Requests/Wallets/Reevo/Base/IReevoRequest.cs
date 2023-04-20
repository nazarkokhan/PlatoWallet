namespace Platipus.Wallet.Api.Application.Requests.Wallets.Reevo.Base;

using Requests.Base;

public interface IReevoRequest : IBaseWalletRequest
{
    public string CallerId { get; init; }
    public string CallerPassword { get; init; }
    public string Action { get; init; }
    public string Username { get; init; }
    public string SessionId { get; init; }
    public string GameSessionId { get; init; }
    public string Key { get; init; }
}