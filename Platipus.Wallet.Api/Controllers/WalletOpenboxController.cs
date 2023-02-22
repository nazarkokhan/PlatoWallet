namespace Platipus.Wallet.Api.Controllers;

using System.Text.Json;
using Abstract;
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
    public async Task<IActionResult> Balance(OpenboxSingleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var casinoId = request.VendorUid switch
            {
                "00000000000000000000000000000001" => "openbox", //TODO move to db
                _ => null
            };
            if (casinoId is null)
                return OpenboxResultFactory.Failure(OpenboxErrorCode.ParameterError).ToActionResult();

            var casino = await _context.Set<Casino>()
                .Where(c => c.Id == casinoId)
                .FirstOrDefaultAsync(cancellationToken);
            if (casino is null)
                return OpenboxResultFactory.Failure(OpenboxErrorCode.ParameterError).ToActionResult();

            var decryptedPayload = OpenboxSecurityPayload.Decrypt(request.Payload, casino.SignatureKey);

            var payloadType = OpenboxHelpers.GetRequestType(request.Method);

            if (payloadType is null)
                return OpenboxResultFactory.Failure(OpenboxErrorCode.ParameterError).ToActionResult();

            var payloadRequestObj = JsonSerializer.Deserialize(decryptedPayload, payloadType, _jsonSerializerOptions);
            if (payloadRequestObj is null)
                return OpenboxResultFactory.Failure(OpenboxErrorCode.ParameterError).ToActionResult();

            HttpContext.Items.Add("OpenboxPayloadRequestObj", payloadRequestObj);
            _logger.LogInformation("Openbox decrypted payload: {OpenboxDecryptedPayload}", payloadRequestObj);

            var responseObj = await _mediator.Send(payloadRequestObj, cancellationToken);
            if (responseObj is not IOpenboxResult response)
                return OpenboxResultFactory.Failure(OpenboxErrorCode.InternalServiceError).ToActionResult();

            return response.ToActionResult();
        }
        catch (Exception e)
        {
            return OpenboxResultFactory.Failure(OpenboxErrorCode.Success, e).ToActionResult();
        }
    }
}