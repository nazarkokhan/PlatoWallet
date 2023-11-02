using Platipus.Wallet.Api.Application.Responses.Sweepium.Base;

namespace Platipus.Wallet.Api.Application.Responses.Sweepium;

public record SweepiumSuccessResponse(
    string TransactionId,
    decimal Balance) : SweepiumCommonResponse;
    
