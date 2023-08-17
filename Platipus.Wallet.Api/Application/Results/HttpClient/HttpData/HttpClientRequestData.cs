namespace Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;

using System.Net.Http.Headers;

public record HttpClientRequestData(
    Uri RequestUri,
    string Method,
    string? Body,
    Dictionary<string, string> Headers);