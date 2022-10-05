namespace PlatipusWallet.Api.Application.Services.GamesApiService;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;

public class RequestSignatureRelegatingHandler : DelegatingHandler
{
    private readonly WalletDbContext _context;

    public RequestSignatureRelegatingHandler(WalletDbContext context)
    {
        _context = context;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
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
        var xRequestSign = Convert.ToHexString(hashedRequest);

        request.Headers.Add("X-REQUEST-SIGN", xRequestSign.ToLower());

        return await base.SendAsync(request, cancellationToken);
    }
}