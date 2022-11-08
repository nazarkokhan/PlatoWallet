namespace Platipus.Wallet.Api.Application.Results.Dafabet;

using Base;

public record DafabetResult : BaseResult<DafabetErrorCode>, IDafabetResult
{
    public DafabetResult()
    {
    }

    public DafabetResult(
        DafabetErrorCode errorCode,
        Exception? exception = null,
        string? description = null) : base(errorCode, exception)
    {
    }
}

// public record OpenboxResult : BaseResult<OpenboxErrorCode>, IOpenboxResult
// {
//     public OpenboxResult()
//     {
//     }
//
//     public OpenboxResult(
//         DatabetErrorCode errorCode,
//         Exception? exception = null,
//         string? description = null) : base(errorCode, exception)
//     {
//     }
// }