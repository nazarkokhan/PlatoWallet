namespace Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;

using Base.WithData;

public interface IEmaraPlayResult<out TData> : IBaseResult<EmaraPlayErrorCode, TData>, IEmaraPlayResult
{
}