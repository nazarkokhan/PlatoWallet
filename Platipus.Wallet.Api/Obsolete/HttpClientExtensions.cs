namespace Platipus.Wallet.Api.Obsolete;

using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Application.Results.Psw;
using Application.Services.PswGamesApi.DTOs.Responses;

[Obsolete]
public static class HttpClientExtensions
{
    public static async Task<IPswResult<T>> GetResponseResult<T>(
        this HttpResponseMessage response,
        JsonSerializerOptions jsonSerializerOptions,
        CancellationToken cancellationToken)
    {
        try
        {
            if (response.StatusCode is not HttpStatusCode.OK)
                return PswResultFactory.Failure<T>(PswErrorCode.Unknown);

            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            var jsonNode = JsonNode.Parse(responseString);
            var responseStatusString = jsonNode?["status"]?.GetValue<string>();

            if (responseStatusString is null)
                return PswResultFactory.Failure<T>(PswErrorCode.Unknown);

            var responseStatus = Enum.Parse<PswStatus>(responseStatusString);

            if (responseStatus is PswStatus.ERROR)
            {
                var errorModel = jsonNode.Deserialize<PswGamesApiErrorResponseDto>(jsonSerializerOptions)!;
                return PswResultFactory.Failure<T>(errorModel.Error);
            }

            var successModel = jsonNode.Deserialize<T>(jsonSerializerOptions)!;

            return PswResultFactory.Success(successModel);
        }
        catch (Exception e)
        {
            return PswResultFactory.Failure<T>(PswErrorCode.Unknown, e);
        }
    }
}