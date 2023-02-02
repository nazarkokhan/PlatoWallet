namespace Platipus.Wallet.Api.Application.Results.EmaraPlay;

using Base;

public interface IEmaraPlayResult : IBaseResult<EmaraPlayErrorCode>
{
    long? Balance { get; }
}