namespace Platipus.Wallet.Api.Controllers.Other;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Results.Psw;
using Abstract;
using Extensions;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions.SecuritySign;
using Infrastructure.Persistence;

[Route("test")]
public class TestController : ApiController
{
    private const string key = "1234567890123456";

    [HttpPost("stringify")]
    public async Task<IActionResult> Stringify(
        [FromBody] object request,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        var result = new { JsonString = JsonSerializer.Serialize(request) };

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

        var rawRequestBytes = (byte[])HttpContext.Items["rawRequestBytes"]!;

        var validSignature = PswRequestSign.Compute(rawRequestBytes, casino.SignatureKey);

        var result = new { Signature = validSignature };

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
        var result = new { Hash = DatabetHash.Compute($"{method}{source}", casino.SignatureKey) };

        return Ok(result);
    }

    [HttpPost("openbox/encrypt-payload")]
    public async Task<IActionResult> OpenboxEncryptPayload(
        string casinoId,
        [FromBody] object request,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        // var casino = await dbContext.Set<Casino>()
        //     .Where(c => c.Id == casinoId)
        //     .Select(
        //         c => new
        //         {
        //             c.SignatureKey
        //         })
        //     .FirstOrDefaultAsync(cancellationToken);
        //
        // if (casino is null)
        //     return ResultFactory.Failure(ErrorCode.InvalidCasinoId).ToActionResult();

        // var signatureKey = casino.SignatureKey;
        var signatureKey = key;

        var serialize = JsonSerializer.Serialize(request);
        var encryptedPayload = OpenboxPayload.Encrypt(serialize, signatureKey);

        var result = new { EncryptedPayload = encryptedPayload };

        return Ok(result);
    }

    [HttpPost("openbox/decrypt-payload")]
    public async Task<IActionResult> OpenboxDecryptPayload(
        string casinoId,
        string request,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        // var casino = await dbContext.Set<Casino>()
        //     .Where(c => c.Id == casinoId)
        //     .Select(
        //         c => new
        //         {
        //             c.SignatureKey
        //         })
        //     .FirstOrDefaultAsync(cancellationToken);
        //
        // if (casino is null)
        //     return ResultFactory.Failure(ErrorCode.InvalidCasinoId).ToActionResult();

        // var signatureKey = casino.SignatureKey;
        var signatureKey = key;

        var decryptedPayload = OpenboxPayload.Decrypt(request, signatureKey);

        var result = new { DecryptedPayload = decryptedPayload };

        return Ok(result);
    }
}