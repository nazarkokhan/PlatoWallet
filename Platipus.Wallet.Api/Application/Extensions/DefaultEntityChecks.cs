namespace Platipus.Wallet.Api.Application.Extensions;

using System.Diagnostics.CodeAnalysis;
using Domain.Entities;

public static class DefaultEntityChecks
{
    public static bool IsNullOrDisabled(
        [NotNullWhen(false)] this User? user,
        [NotNullWhen(true)] out IResult? userNullOrDisabledResult)
    {
        userNullOrDisabledResult = user switch
        {
            null => ResultFactory.Failure(ErrorCode.UserNotFound),
            { IsDisabled: true } => ResultFactory.Failure(ErrorCode.UserIsDisabled),
            _ => null
        };

        return userNullOrDisabledResult is not null;
    }

    public static bool IsNullOrDisabled<TData>(
        [NotNullWhen(false)] this User? user,
        [NotNullWhen(true)] out IResult<TData>? userNullOrDisabledResult)
    {
        userNullOrDisabledResult = user switch
        {
            null => ResultFactory.Failure<TData>(ErrorCode.UserNotFound),
            { IsDisabled: true } => ResultFactory.Failure<TData>(ErrorCode.UserIsDisabled),
            _ => null
        };

        return userNullOrDisabledResult is not null;
    }
}