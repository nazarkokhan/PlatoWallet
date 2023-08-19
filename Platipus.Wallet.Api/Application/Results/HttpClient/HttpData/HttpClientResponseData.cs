namespace Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;

using System.Net;

public record HttpClientResponseData(
    HttpStatusCode StatusCode,
    string? Body,
    Dictionary<string, string> Headers);