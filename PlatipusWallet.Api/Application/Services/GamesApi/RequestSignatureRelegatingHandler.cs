namespace PlatipusWallet.Api.Application.Services.GamesApi;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;
using StartupSettings;

public class RequestSignatureRelegatingHandler : DelegatingHandler
{
    private readonly WalletDbContext _context;
    private readonly ILogger<RequestSignatureRelegatingHandler> _logger;

    public RequestSignatureRelegatingHandler(WalletDbContext context, ILogger<RequestSignatureRelegatingHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        //TODO move logging to filter
        var requestBytes = await request.Content!.ReadAsByteArrayAsync(cancellationToken);

        const string casinoPropertyName = "casino_id";
        var casinoId = JsonNode.Parse(requestBytes)?[casinoPropertyName]?.GetValue<string>();

        if (casinoId is null)
            throw new ArgumentException(casinoPropertyName);

        var casino = await _context.Set<Casino>()
            .Where(c => c.Id == casinoId)
            .Select(
                c => new
                {
                    c.SignatureKey
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            throw new NullReferenceException("Casino not found");

        var signatureKeyBytes = Encoding.UTF8.GetBytes(casino.SignatureKey);
        var hashedRequest = HMACSHA256.HashData(signatureKeyBytes, requestBytes);
        var xRequestSign = Convert.ToHexString(hashedRequest).ToLower();

        request.Headers.Add(PswHeaders.XRequestSign, xRequestSign);

        _logger.LogInformation(
            "GamesApi Request: {GamesApiRequest}, X-REQUEST-SIGN: {GamesApi.RequestSign}", 
            Encoding.UTF8.GetString(requestBytes),
            xRequestSign);

        var response = await base.SendAsync(request, cancellationToken);

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogInformation("GamesApi Request: {GamesApiResponse}", responseString);

        return response;
    }
}