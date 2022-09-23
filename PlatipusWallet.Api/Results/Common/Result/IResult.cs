namespace PlatipusWallet.Api.Results.Common.Result;

public interface IResult
{
    bool IsSuccess { get; }

    bool IsFailure { get; }

    public ErrorCode ErrorCode { get; }
    
    string ErrorDescription { get; set; }

    Exception? Exception { get; }

}