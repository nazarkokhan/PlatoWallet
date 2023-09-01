namespace Platipus.Wallet.Api.Application.Requests.Wallets.Parimatch.Responses;

using Humanizer;
using JetBrains.Annotations;
using Platipus.Wallet.Api.Application.Results.Nemesis;

[PublicAPI]
public class ParimatchErrorResponse
{
    public ParimatchErrorResponse(NemesisErrorCode errorCode)
    {
        Id = $"{Guid.NewGuid()}.{Guid.NewGuid()}";
        Code = errorCode.ToString().Dasherize().ToUpper();
        Messages = new[] { errorCode.Humanize() };
    }

    public string Id { get; init; }
    public string Code { get; init; }
    public string[] Messages { get; init; }
}

public record ParimatchCommonResponse(
    string Country,
    string Currency,
    decimal Balance,
    string DisplayName,
    string PlayerId
);

