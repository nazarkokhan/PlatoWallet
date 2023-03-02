namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix.Base;

public interface IEveryMatrixRequest : IEveryMatrixBaseRequest
{
    public string Hash { get; init; }

    public string Token { get; init; }
}