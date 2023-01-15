namespace Platipus.Wallet.Api.Controllers.Other;

using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Abstract;
using Application.Extensions;
using Application.Results.Hub88;
using Application.Results.Sw;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

[Route("test")]
public class TestController : RestApiController
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
        var result = new {JsonString = JsonSerializer.Serialize(request)};

        return Ok(result);
    }

    [HttpPost("softswiss/get-hash-body")]
    [HttpPost("psw/get-hash-body")]
    public async Task<IActionResult> PsvSignature(
        string casinoId,
        [FromBody] object request,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var casino = await dbContext.Set<Casino>()
            .Where(c => c.Id == casinoId)
            .Select(c => new {c.SignatureKey})
            .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return PswResultFactory.Failure(PswErrorCode.InvalidCasinoId).ToActionResult();

        var rawRequestBytes = (byte[]) HttpContext.Items["rawRequestBytes"]!;

        var validSignature = PswRequestSign.Compute(rawRequestBytes, casino.SignatureKey);

        var result = new {Signature = validSignature};

        return Ok(result);
    }

    [HttpPost("dafabet/get-hash-body")]
    public async Task<IActionResult> DafabetSignature(
        [FromBody] Dictionary<string, object> request,
        [FromQuery] string method,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var casino = await dbContext.Set<Casino>()
            .Where(c => c.Provider == CasinoProvider.Dafabet)
            .Select(c => new {c.SignatureKey})
            .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return PswResultFactory.Failure(PswErrorCode.InvalidCasinoId).ToActionResult();

        var sourceValues = request.Values.Select(
            v =>
            {
                if (bool.TryParse(v.ToString(), out var boolV))
                    return boolV.ToString()?.ToLower();
                return v.ToString();
            });

        var source = string.Concat(sourceValues);
        var result = new {Hash = DatabetHash.Compute($"{method}{source}", casino.SignatureKey)};

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

        var result = new {EncryptedPayload = encryptedPayload};

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

        var result = new {DecryptedPayload = decryptedPayload};

        return Ok(result);
    }

    [HttpGet("openbox/unix-now")]
    public async Task<IActionResult> OpenboxUnixNow(DateTime? time, CancellationToken cancellationToken)
    {
        var unixNow = (time ?? DateTimeOffset.UtcNow).ToUnixTimeMilliseconds();

        var result = new {UnixNow = unixNow};

        return Ok(result);
    }

    [HttpPost("hub88/get-hash-body")]
    public async Task<IActionResult> Hub88Signature(
        string casinoId,
        [FromBody] object request,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var casino = await dbContext.Set<Casino>()
            .Where(c => c.Id == casinoId)
            .Select(c => new {c.SignatureKey})
            .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return Hub88ResultFactory.Failure(Hub88ErrorCode.RS_ERROR_WRONG_SYNTAX).ToActionResult();

        var rawRequestBytes = (byte[]) HttpContext.Items["rawRequestBytes"]!;

        var validSignature = Hub88RequestSign.Compute(rawRequestBytes, Hub88RequestSign.PrivateKeyForWalletItself);

        var isValid = Hub88RequestSign.IsValidSign(validSignature, rawRequestBytes, casino.SignatureKey);

        var result = new {Signature = validSignature};

        return Ok(result);
    }

    [HttpPost("sw/get-hash")]
    public async Task<IActionResult> SwHash(
        string userName,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Set<User>()
            .Where(c => c.UserName == userName)
            .Select(
                c => new
                {
                    UserId = c.SwUserId,
                    Casino = new
                    {
                        ProviderId = c.Casino.SwProviderId,
                        c.Casino.SignatureKey,
                    }
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return SwResultFactory.Failure(SwErrorCode.UserNotFound).ToActionResult();

        var validSignature = user.Map(u => SwRequestHash.Compute(u.Casino.ProviderId ?? 0, u.UserId ?? 0, u.Casino.SignatureKey));

        var result = new {Signature = validSignature};

        return Ok(result);
    }

    [HttpPost("sw/get-md5")]
    public async Task<IActionResult> SwMd5(
        string userName,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Set<User>()
            .Where(c => c.UserName == userName)
            .Select(
                c => new
                {
                    UserId = c.SwUserId,
                    Casino = new
                    {
                        ProviderId = c.Casino.SwProviderId,
                        c.Casino.SignatureKey,
                    }
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return SwResultFactory.Failure(SwErrorCode.UserNotFound).ToActionResult();

        var validSignature = user.Map(u => SwRequestMd5.Compute(u.Casino.ProviderId ?? 0, u.UserId ?? 0, u.Casino.SignatureKey));

        var result = new {Signature = validSignature};

        return Ok(result);
    }

    [HttpPost("softbet/hash")]
    public async Task<IActionResult> SoftBet(
        string userName,
        object request,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Set<User>()
            .Where(c => c.UserName == userName)
            .Select(
                c => new
                {
                    UserId = c.SwUserId,
                    Casino = new
                    {
                        ProviderId = c.Casino.SwProviderId,
                        c.Casino.SignatureKey,
                    }
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return SwResultFactory.Failure(SwErrorCode.UserNotFound).ToActionResult();

        var rawRequestBytes = (byte[]) HttpContext.Items["rawRequestBytes"]!;

        var validSignature = user.Map(u => SoftBetRequestHash.Compute(rawRequestBytes, u.Casino.SignatureKey));

        var result = new {Signature = validSignature};

        return Ok(result);
    }

    [HttpPost("softbet/dynamic")]
    public async Task<IActionResult> SoftBet(
        SomeTest request,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return Ok(request);
    }

    [HttpPost("everymatrix/hash")]
    public async Task<IActionResult> EveryMatrix(ActionType actionType, Guid userId)
    {
        var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.Id == userId);

        var password = user.Password;

        var dateTime = DateTime.UtcNow.ToString("yyyy:MM:dd:HH", CultureInfo.InvariantCulture);

        var md5Hash = MD5.Create()
            .ComputeHash(Encoding.UTF8.GetBytes($"NameOfMethod({actionType})Time({dateTime})password({password})"));

        var validHash = Convert.ToHexString(md5Hash);

        return Ok(validHash);
    }

    [HttpPost("emaraplay/hash")]
    public async Task<IActionResult> EmaraPlay(object request)
    {
        var bodyToHash = JsonConvert.SerializeObject(request);
        var hashedBody = Encoding.UTF8.GetBytes(bodyToHash);
        var secretBytes = Encoding.UTF8.GetBytes("EmaraPlaySecret");
        return Ok(HMACSHA512.HashData(secretBytes, hashedBody));

    }

    [HttpPost("betflag/hash")]
    public async Task<IActionResult> Betflag(long timestamp, string sessionId)
    {
        var validHash = BetflagRequestHash.Compute( sessionId, timestamp);

        return Ok(validHash);
    }

    [HttpPost("betconstruct/hash")]
    public async Task<IActionResult> BetConstruct(string data, DateTime time)
    {
        var validHash = BetConstructRequestHash.Compute(time.ToString(CultureInfo.InvariantCulture), data);
        return Ok(validHash);
    }

    public record SomeTest(JsonNode SomeProp);
}