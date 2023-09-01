namespace Platipus.Wallet.Api.Application.Services.PswGameApi.Requests;

using System.ComponentModel;

public record PswGameSessionGameApiRequest(
    string CasinoId,
    string SessionId,
    string User,
    string Currency,
    string Game,
    string Locale,
    string Lobby,
    [property: DefaultValue("url")] string LaunchMode,
    int Rci) : IPswGameApiBaseRequest;