namespace Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;

public record HttpClientRequestData(
    Uri RequestUri,
    string Method,
    string? Body,
    Dictionary<string, string> Headers);