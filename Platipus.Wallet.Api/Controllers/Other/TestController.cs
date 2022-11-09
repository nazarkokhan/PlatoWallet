namespace Platipus.Wallet.Api.Controllers.Other;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Results.Psw;
using Abstract;
using Application.Results.Openbox;
using Extensions;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions.SecuritySign;
using Infrastructure.Persistence;

[Route("test")]
public class TestController : ApiController
{
    private readonly WalletDbContext _context;

    public TestController(WalletDbContext context)
    {
        _context = context;
    }

    [HttpPost("stringify")]
    public async Task<IActionResult> Stringify([FromBody] object request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        var result = new
        {
            JsonString = JsonSerializer.Serialize(request)
        };

        return Ok(result);
    }

    [HttpPost("psw/get-hash-body")]
    public async Task<IActionResult> PsvSignature(
        string casinoId,
        [FromBody] object request,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var casino = await dbContext.Set<Casino>()
            .Where(c => c.Id == casinoId)
            .Select(
                c => new
                {
                    c.SignatureKey
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return ResultFactory.Failure(ErrorCode.InvalidCasinoId).ToActionResult();

        var rawRequestBytes = (byte[]) HttpContext.Items["rawRequestBytes"]!;

        var validSignature = PswRequestSign.Compute(rawRequestBytes, casino.SignatureKey);

        var result = new
        {
            Signature = validSignature
        };

        return Ok(result);
    }

    [HttpPost("dafabet/get-hash-body")]
    public async Task<IActionResult> DafabetSignature(
        [FromBody] Dictionary<string, string> request,
        [FromQuery] string method,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var casino = await dbContext.Set<Casino>()
            .Where(c => c.Provider == CasinoProvider.Dafabet)
            .Select(
                c => new
                {
                    c.SignatureKey
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return ResultFactory.Failure(ErrorCode.InvalidCasinoId).ToActionResult();

        var source = string.Concat(request.Values);
        var result = new
        {
            Hash = DatabetHash.Compute($"{method}{source}", casino.SignatureKey)
        };

        return Ok(result);
    }

    [HttpPost("openbox/encrypt-payload")]
    public async Task<IActionResult> OpenboxEncryptPayload(
        string vendorUid,
        [FromBody] object request,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var casinoId = vendorUid switch
        {
            "00000000000000000000000000000001" => "openbox",
            _ => null
        };
        if (casinoId is null)
            return OpenboxResultFactory.Failure(OpenboxErrorCode.ParameterError).ToActionResult();

        var casino = await _context.Set<Casino>()
            .Where(c => c.Id == casinoId)
            .FirstOrDefaultAsync(cancellationToken);
        if (casino is null)
            return OpenboxResultFactory.Failure(OpenboxErrorCode.ParameterError).ToActionResult();

        var signatureKey = casino.SignatureKey;

        var serialize = JsonSerializer.Serialize(request);
        var encryptedPayload = OpenboxPayload.Encrypt(serialize, signatureKey);

        var result = new
        {
            EncryptedPayload = encryptedPayload
        };

        return Ok(result);
    }

    [HttpPost("openbox/decrypt-payload")]
    public async Task<IActionResult> OpenboxDecryptPayload(
        string vendorUid,
        string request,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var casinoId = vendorUid switch
        {
            "00000000000000000000000000000001" => "openbox",
            _ => null
        };
        if (casinoId is null)
            return OpenboxResultFactory.Failure(OpenboxErrorCode.ParameterError).ToActionResult();

        var casino = await _context.Set<Casino>()
            .Where(c => c.Id == casinoId)
            .FirstOrDefaultAsync(cancellationToken);
        if (casino is null)
            return OpenboxResultFactory.Failure(OpenboxErrorCode.ParameterError).ToActionResult();

        var signatureKey = casino.SignatureKey;

        var decryptedPayload = OpenboxPayload.Decrypt(request, signatureKey);

        var result = new
        {
            DecryptedPayload = decryptedPayload
        };

        return Ok(result);
    }

    [HttpGet("openbox/unix-now")]
    public async Task<IActionResult> OpenboxUnixNow(CancellationToken cancellationToken)
    {
        var unixNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var result = new
        {
            UnixNow = unixNow
        };

        return Ok(result);
    }
}