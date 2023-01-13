namespace Platipus.Wallet.Api.Application.Results.EmaraPlay;

using Platipus.Wallet.Api.Application.Results.Base;

public interface IEmaraPlayResult : IBaseResult<EmaraPlayErrorCode>
{
    long? Balance { get; }
}