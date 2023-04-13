namespace Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;

using System.Net.Http.Headers;

public record HttpClientRequestData(
    Uri RequestUri,
    HttpMethod Method,
    string? Body,
    HttpHeadersNonValidated Headers);