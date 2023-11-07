using System.Text.Json.Serialization;

namespace Platipus.Wallet.Api.Application.Responses.Sweepium.Base;

public record SweepiumCommonResponse(
    bool Result = true);