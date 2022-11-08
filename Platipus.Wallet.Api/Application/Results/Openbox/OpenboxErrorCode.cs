namespace Platipus.Wallet.Api.Application.Results.Openbox;

public enum OpenboxErrorCode
{
    Success = 0,
    InternalServiceError = 1,
    TokenRelatedErrors = 2,
    ValidationError = 3,
    ParameterError = 4,
}