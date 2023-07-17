namespace Platipus.Wallet.Api.Application.Results.Evenbet;

using Base;

public interface IEvenbetResult : IBaseResult<EvenbetErrorCode>
{
    public int Balance { get; }
    
    public string Timestamp { get; }
}