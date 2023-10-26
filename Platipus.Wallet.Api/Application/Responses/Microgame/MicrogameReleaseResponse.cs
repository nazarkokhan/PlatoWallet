namespace Platipus.Wallet.Api.Application.Responses.Microgame;

using Base;
using Results.Microgame;

public sealed record MicrogameReleaseResponse(
    string ExternalTransactionId,
    string Currency,
    decimal SessionBalance) : MicrogameCommonOperationsResponse(
    MicrogameStatusCode.OK,
    ExternalTransactionId,
    Currency,
    SessionBalance);