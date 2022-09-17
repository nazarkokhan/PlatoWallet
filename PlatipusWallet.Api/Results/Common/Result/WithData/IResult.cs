namespace PlatipusWallet.Api.Results.Common.Result.WithData;

public interface IResult<out T> : IResult
{
    T Data { get; }
}