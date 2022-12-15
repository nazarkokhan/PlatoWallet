namespace Platipus.Wallet.Api.Application.Services.GamesGlobalGamesApi;

using System.Text.Json.Nodes;

public class GamesGlobalGamesApiClient : IGamesGlobalGamesApiClient
{
    private readonly HttpClient _httpClient;

    public GamesGlobalGamesApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResult<string?>> GetLaunchUrlAsync(Guid token, string game, CancellationToken cancellationToken = default)
    {
        try
        {
            var requestBody = new
            {
                Token = token,
                Lobby = "",
                Locale = "en",
                Game = game
            };
            var jsonContent = JsonContent.Create(requestBody);

            var httpResponse = await _httpClient.PostAsync("", jsonContent, cancellationToken);

            var responseString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            var responseJsonNode = JsonNode.Parse(responseString);

            var error = responseJsonNode?["error"]?.GetValue<string>();

            if (error is null)
                return ResultFactory.Failure<string?>(ErrorCode.InvalidSignature);

            var url = responseJsonNode?["url"]?.GetValue<string>();

            return ResultFactory.Success(url);
        }
        catch (Exception e)
        {
            return ResultFactory.Failure<string?>(ErrorCode.Unknown, e);
        }
    }
}