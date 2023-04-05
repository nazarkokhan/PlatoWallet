namespace Platipus.Wallet.Api.Controllers;

using System.Text.Json;
using Abstract;
using Application.Requests.Wallets.Openbox;
using Application.Requests.Wallets.Openbox.Base;
using Application.Requests.Wallets.Openbox.Base.Response;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StartupSettings;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;

[Route("wallet/openbox/")]
[MockedErrorActionFilter(Order = 1)]
[JsonSettingsName(nameof(CasinoProvider.Openbox))]
public class WalletOpenboxController : RestApiController
{
    private readonly IMediator _mediator;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly WalletDbContext _context;
    private readonly ILogger<WalletOpenboxController> _logger;

    public WalletOpenboxController(
        IMediator mediator,
        IOptionsMonitor<JsonOptions> options,
        WalletDbContext context,
        ILogger<WalletOpenboxController> logger)
    {
        _mediator = mediator;
        _jsonSerializerOptions = options.Get(nameof(CasinoProvider.Openbox)).JsonSerializerOptions;
        _context = context;
        _logger = logger;
    }

    [HttpPost("main")]
    [ProducesResponseType(typeof(OpenboxSingleResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Main(OpenboxSingleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var casino = await _context.Set<Casino>()
                .Where(
                    c => c.Provider == CasinoProvider.Openbox
                      && c.Params.OpenboxVendorUid == request.VendorUid)
                .Select(
                    c => new
                    {
                        c.Id,
                        c.SignatureKey
                    })
                .FirstOrDefaultAsync(cancellationToken);
            if (casino is null)
                return OpenboxResultFactory.Failure(OpenboxErrorCode.ParameterError).ToActionResult();

            var decryptedPayloadJson = OpenboxSecurityPayload.Decrypt(request.Payload, casino.SignatureKey);
            var payloadType = OpenboxHelpers.GetRequestType(request.Method);

            if (payloadType is null)
                return OpenboxResultFactory.Failure(OpenboxErrorCode.ParameterError).ToActionResult();

            var payloadRequestObj = JsonSerializer.Deserialize(decryptedPayloadJson, payloadType, _jsonSerializerOptions);
            if (payloadRequestObj is not IOpenboxBaseRequest decryptedPayload)
                return OpenboxResultFactory.Failure(OpenboxErrorCode.ParameterError).ToActionResult();

            var session = await _context.Set<Session>()
                .Where(s => s.Id == decryptedPayload.Token)
                .Select(
                    s => new
                    {
                        s.ExpirationDate,
                        s.IsTemporaryToken,
                        UserCasinoId = s.User.CasinoId
                    })
                .FirstOrDefaultAsync(cancellationToken);
            if (session is null || session.ExpirationDate < DateTime.UtcNow)
                return OpenboxResultFactory.Failure(OpenboxErrorCode.TokenRelatedErrors).ToActionResult();

            var isAuthRequest = payloadRequestObj is OpenboxVerifyPlayerRequest;
            if (isAuthRequest ? !session.IsTemporaryToken : session.IsTemporaryToken)
                return OpenboxResultFactory.Failure(OpenboxErrorCode.TokenRelatedErrors).ToActionResult();

            if (casino.Id != session.UserCasinoId)
                return OpenboxResultFactory.Failure(OpenboxErrorCode.ParameterError).ToActionResult();

            HttpContext.Items.Add(HttpContextItems.OpenboxPayloadRequestObj, decryptedPayload);
            _logger.LogInformation("Openbox decrypted payload: {OpenboxDecryptedPayload}", decryptedPayload);

            var responseObj = await _mediator.Send(decryptedPayload, cancellationToken);
            if (responseObj is not IOpenboxResult response)
                return OpenboxResultFactory.Failure(OpenboxErrorCode.InternalServiceError).ToActionResult();

            return response.ToActionResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Openbox unexpected exception");
            return OpenboxResultFactory.Failure(OpenboxErrorCode.InternalServiceError, e).ToActionResult();
        }
    }
}

[Route("wallet/private/openbox")]
[JsonSettingsName(nameof(CasinoProvider.Everymatrix))]
public class WalletOpenboxPrivateController : RestApiController
{
    [HttpPost("get-security-value")]
    public async Task<IActionResult> GetSecurityValue(
        string vendorUid,
        [FromBody] JsonDocument request,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var casino = await dbContext.Set<Casino>()
            .Where(
                c => c.Provider == CasinoProvider.Openbox
                  && c.Params.OpenboxVendorUid == vendorUid)
            .Select(
                c => new
                {
                    c.Id,
                    c.SignatureKey
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return ResultFactory.Failure(ErrorCode.CasinoNotFound).ToActionResult();

        var signatureKey = casino.SignatureKey;

        var serialize = JsonSerializer.Serialize(request);
        var securityValue = OpenboxSecurityPayload.Encrypt(serialize, signatureKey);

        return Ok(securityValue);
    }

    [HttpGet("decrypt-payload")]
    public async Task<IActionResult> OpenboxDecryptPayload(
        string vendorUid,
        string requestPayload,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var casino = await dbContext.Set<Casino>()
            .Where(
                c => c.Provider == CasinoProvider.Openbox
                  && c.Params.OpenboxVendorUid == vendorUid)
            .Select(
                c => new
                {
                    c.Id,
                    c.SignatureKey
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return ResultFactory.Failure(ErrorCode.CasinoNotFound).ToActionResult();

        var signatureKey = casino.SignatureKey;

        var decryptedPayload = OpenboxSecurityPayload.Decrypt(requestPayload, signatureKey);

        return Ok(decryptedPayload);
    }
}