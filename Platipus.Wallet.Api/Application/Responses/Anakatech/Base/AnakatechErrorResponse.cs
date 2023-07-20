﻿namespace Platipus.Wallet.Api.Application.Responses.Anakatech.Base;

public sealed record AnakatechErrorResponse(
    bool Success,
    int Balance,
    string? ErrorCode) : AnakatechBaseResponse(
    Success,
    Balance,
    ErrorCode);