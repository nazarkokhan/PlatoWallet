namespace Platipus.Wallet.Api.Application.Requests.Base.Common.Page;

public record PageRequest(
    int Size = 10,
    int Number = 1)
{
    public static readonly PageRequest Default = new();
}