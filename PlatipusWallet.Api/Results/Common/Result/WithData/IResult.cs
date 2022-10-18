namespace PlatipusWallet.Api.Results.Common.Result.WithData;

public interface IResult<out TData> : IBaseResult<ErrorCode, TData>, IResult
{
}

public interface IDatabetResult<out TData> : IBaseResult<DatabetErrorCode, TData>, IDatabetResult
{
}

public interface IBaseResult<out TError, out TData> : IBaseResult<TError>
{
    TData Data { get; }
    
    IBaseResult<TNewError, TNewData> ConvertResult<TNewError, TNewData>(TNewError error, TNewData data);
}