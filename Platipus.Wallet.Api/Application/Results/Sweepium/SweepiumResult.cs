using Platipus.Wallet.Api.Application.Results.Base;
namespace Platipus.Wallet.Api.Application.Results.Sweepium;

public sealed record SweepiumResult : BaseResult<SweepiumErrorCode>, ISweepiumResult
{
      public SweepiumResult()
      {
            ErrorDescription = string.Empty;
      }
      
      public SweepiumResult(
            SweepiumErrorCode errorCode,
            Exception? exception = null,
            string? description = null)
            : base(errorCode, exception)
      {
            ErrorDescription = description ?? string.Empty;
      }
      

      public string ErrorDescription { get; set; }
}