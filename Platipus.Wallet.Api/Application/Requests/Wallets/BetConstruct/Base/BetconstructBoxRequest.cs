namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base;

public record BetconstructBoxRequest<TRequestData>(TRequestData Data, string Time, string Hash)
    : IBetconstructBoxRequest<TRequestData>
    where TRequestData : IBetconstructRequest
{
    public string Hash { get; } = Hash;
    public string Time { get; } = Time;
    public TRequestData Data { get; } = Data;
}

public interface IBetconstructBoxRequest<out TRequestData>
    where TRequestData : IBetconstructRequest
{
    public string Hash { get; }
    public string Time { get; }
    public TRequestData Data { get; }
}