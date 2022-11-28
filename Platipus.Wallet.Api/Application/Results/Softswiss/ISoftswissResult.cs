namespace Platipus.Wallet.Api.Application.Results.Softswiss;

using Base;

public interface ISoftswissResult : IBaseResult<SoftswissErrorCode>
{
    long? Balance { get; }
}