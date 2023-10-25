namespace Platipus.Wallet.Api.Application.Responses.Microgame;

using Base;
using Results.Microgame;

public sealed record MicrogameGetBalanceResponse(
    string Currency,
    decimal SessionBalance) : MicrogameCommonResponse(MicrogameStatusCode.OK);