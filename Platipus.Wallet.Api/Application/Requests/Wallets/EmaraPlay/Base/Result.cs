namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;

//TODO As we ha many contracts use longer names like IEmaraPlayBaseResponseNew
public record Result(
    string? User = null,
    string? Username = null,
    string? Currency = null,
    string? Balance = null,
    string? Bonus = null,
    string? Country = null,
    string? Jurisdiction = null);

public interface IEmaraPlayBaseResponseNew
{
    //TODO leave only common parameters, dont use null properties if they are required
    public string User { get; init; }
    // public string Username { get; init; }
    public string Currency { get; init; }
    public string Balance { get; init; }
    public string Bonus { get; init; }
    // public string Country { get; init; } //it is not common parameter
    public string Jurisdiction { get; init; }
}

//TODO example of your mediatr response
public record EmaraPlayAuthenticateResponse(
    string User,
    string Currency,
    string Balance,
    string Bonus,
    string Jurisdiction) : IEmaraPlayBaseResponseNew;