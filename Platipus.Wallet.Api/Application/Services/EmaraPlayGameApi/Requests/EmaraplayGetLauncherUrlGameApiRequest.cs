namespace Platipus.Wallet.Api.Application.Services.EmaraPlayGameApi.Requests;

public sealed record EmaraplayGetLauncherUrlGameApiRequest(
    string Operator,
    string Game,
    string Mode,
    string Lang,
    string Channel,
    string Jurisdiction,
    string Currency,
    string Ip,
    string? Token = null,
    string? User = null,
    string? Lobby = null,
    string? Cashier = null);