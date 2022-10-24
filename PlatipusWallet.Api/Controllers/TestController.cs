namespace PlatipusWallet.Api.Controllers;

using System.Security.Cryptography;
using System.Text;
using Abstract;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using Results.Common.Result.Factories;

[Route("test")]
public class TestController : ApiController
{
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

        var signatureKeyBytes = Encoding.UTF8.GetBytes(casino.SignatureKey);
        var hmac = HMACSHA256.HashData(signatureKeyBytes, rawRequestBytes);
        var validSignature = Convert.ToHexString(hmac);

        var result = new { Signature = validSignature.ToLower() };

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
}