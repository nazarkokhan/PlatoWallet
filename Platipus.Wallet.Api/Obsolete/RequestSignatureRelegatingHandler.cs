namespace Platipus.Wallet.Api.Obsolete;

using System.Text;
using System.Text.Json.Nodes;
using Domain.Entities;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

[Obsolete]
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
        var requestBytes = await request.Content!.ReadAsByteArrayAsync(cancellationToken);

        const string casinoPropertyName = "casino_id";
        var casinoId = JsonNode.Parse(requestBytes)?[casinoPropertyName]?.GetValue<string>();

        if (casinoId is not null)
        {
            var casino = await _context.Set<Casino>()
                .Where(c => c.Id == casinoId)
                .Select(
                    c => new
                    {
                        c.Provider,
                        c.SignatureKey
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (casino is null)
                throw new NullReferenceException("Casino not found");

            var xRequestSign = PswRequestSign.Compute(requestBytes, casino.SignatureKey);

            request.Headers.Add(PswHeaders.XRequestSign, xRequestSign);

            _logger.LogInformation(
                "GamesApi Request: {GamesApiRequest}, X-REQUEST-SIGN: {GamesApiRequestSign}",
                Encoding.UTF8.GetString(requestBytes),
                xRequestSign);
        }

        var response = await base.SendAsync(request, cancellationToken);

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogInformation("GamesApi Request: {GamesApiResponse}", responseString);

        return response;
    }
}