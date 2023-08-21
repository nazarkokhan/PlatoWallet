namespace Platipus.Wallet.Api.Application.Requests.Wallets.Nemesis.Responses;

using Humanizer;
using JetBrains.Annotations;
using Results.Nemesis;

[PublicAPI]
public class NemesisErrorResponse
{
    public NemesisErrorResponse(NemesisErrorCode errorCode)
    {
        Id = $"{Guid.NewGuid()}.{Guid.NewGuid()}";
        Code = errorCode.ToString().Dasherize().ToUpper();
        Messages = new[] { errorCode.Humanize() };
    }

    public string Id { get; init; }
    public string Code { get; init; }
    public string[] Messages { get; init; }
}