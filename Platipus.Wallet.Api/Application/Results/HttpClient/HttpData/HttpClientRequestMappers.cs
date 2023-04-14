namespace Platipus.Wallet.Api.Application.Results.HttpClient.HttpData;

public static class HttpClientRequestMappers
{
    public static async Task<HttpClientRequest> MapToHttpClientResponseAsync(
        this HttpResponseMessage responseMsg,
        CancellationToken cancellationToken)
    {
        var requestMsg = responseMsg.RequestMessage!;

        var requestMsgContent = requestMsg.Content;
        var requestBody = requestMsgContent is not null
            ? await requestMsgContent.ReadAsStringAsync(cancellationToken)
            : null;
        if (string.IsNullOrWhiteSpace(requestBody))
            requestBody = null;

        var requestDto = new HttpClientRequestData(
            requestMsg.RequestUri!,
            requestMsg.Method,
            requestBody,
            requestMsg.Headers.NonValidated);

        var responseMsgContent = responseMsg.Content;
        var responseBody = await responseMsgContent.ReadAsStringAsync(cancellationToken);
        // if (string.IsNullOrWhiteSpace(responseBody))
        //     responseBody = null;

        var responseDto = new HttpClientResponseData(
            responseMsg.StatusCode,
            responseBody,
            responseMsg.Headers.NonValidated);

        return new HttpClientRequest(requestDto, responseDto);
    }
}