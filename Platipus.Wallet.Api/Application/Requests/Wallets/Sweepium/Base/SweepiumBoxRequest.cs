namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Base;

public record SweepiumBoxRequest<TRequestData>(
        TRequestData Data,
        string Time,
        string Hash)
    : ISweepiumBoxRequest<TRequestData>
    where TRequestData : ISweepiumRequest
{
    public string Hash { get; } = Hash;
    public string Time { get; } = Time;
    public TRequestData Data { get; } = Data;
}