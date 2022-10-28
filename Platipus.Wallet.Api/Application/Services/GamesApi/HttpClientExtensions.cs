namespace Platipus.Wallet.Api.Application.Services.GamesApi;

using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using DTOs.Base;
using Results.Common;
using Results.Common.Result.Factories;
using Results.Common.Result.WithData;
using Results.External.Enums;

public static class HttpClientExtensions
{
    public static async Task<IResult<T>> GetResponseResult<T>(
        this HttpResponseMessage response,
        JsonSerializerOptions jsonSerializerOptions,
        CancellationToken cancellationToken)
    {
        try
        {
            if (response.StatusCode is not HttpStatusCode.OK)
                return ResultFactory.Failure<T>(ErrorCode.Unknown);

            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            var jsonNode = JsonNode.Parse(responseString);
            var responseStatusString = jsonNode?["status"]?.GetValue<string>();

            if (responseStatusString is null)
                return ResultFactory.Failure<T>(ErrorCode.Unknown);

            var responseStatus = Enum.Parse<Status>(responseStatusString);

            if (responseStatus is Status.ERROR)
            {
                var errorModel = jsonNode.Deserialize<BaseGamesApiErrorResponseDto>(jsonSerializerOptions)!;
                return ResultFactory.Failure<T>(errorModel.Error);
            }

            var successModel = jsonNode.Deserialize<T>(jsonSerializerOptions)!;

            return ResultFactory.Success(successModel);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<T>(ErrorCode.Unknown, e);
        }
    }
}