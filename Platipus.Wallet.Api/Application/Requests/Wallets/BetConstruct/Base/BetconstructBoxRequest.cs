namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base;

public record BetconstructBoxRequest<TRequestData>(TRequestData Data, string Time, string Hash)
    where TRequestData : IBetconstructRequest
{
    public string Hash { get; } = Hash;
    public string Time { get; } = Time;
    public TRequestData Data { get; } = Data;
}