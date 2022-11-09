namespace Platipus.Wallet.Api.Controllers;

using System.Text.Json;
using Abstract;
using Application.Requests.Wallets.Openbox;
using Application.Requests.Wallets.Openbox.Base;
using Application.Requests.Wallets.Openbox.Base.Response;
using Application.Results.Openbox;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StartupSettings.ControllerSpecificJsonOptions;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

[Route("wallet/openbox/api/v1")]
[JsonSettingsName(nameof(CasinoProvider.Openbox))]
public class WalletOpenboxController : ApiController
{
    private const string VerifyPlayer = "e80355cb500e491f8ec067e54ba4e1e4";
    private const string GetPlayerInformation = "21a28abbc3744c47b03113e27b465475";
    private const string GetPlayerBalance = "96e7fc4ac82c4629b23da5296d25ec61";
    private const string MoneyTransactions = "b02cb70be50e41468aabf8d32237a3d4";
    private const string CancelTransaction = "68f28eb3c925488e95eb470670bc8827";
    private const string Logout = "562db5a013634b7195b1d0c650c414cf";
    private const string KeepTokenAlive = "09caee7b676f4c1c95050cd2e0bb5074";

    private readonly IMediator _mediator;
    private readonly IOptions<JsonOptions> _options; //TODO try get from di
    private readonly WalletDbContext _context;

    public WalletOpenboxController(IMediator mediator, IOptions<JsonOptions> options, WalletDbContext context)
    {
        _mediator = mediator;
        _options = options;
        _context = context;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OpenboxSingleResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(OpenboxSingleRequest request, CancellationToken cancellationToken)
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

        var decryptedPayload = OpenboxPayload.Decrypt(request.Payload, casino.SignatureKey);

        var payloadType = request.Method switch
        {
            VerifyPlayer => typeof(OpenboxVerifyPlayerRequest),
            GetPlayerInformation => typeof(OpenboxGetPlayerInfoRequest),
            GetPlayerBalance => typeof(OpenboxBalanceRequest),
            MoneyTransactions => typeof(OpenboxMoneyTransactionRequest),
            CancelTransaction => typeof(OpenboxCancelTransactionRequest),
            Logout => typeof(OpenboxLogoutRequest),
            KeepTokenAlive => typeof(OpenboxKeepTokenAliveRequest),
            _ => null
        };
        if (payloadType is null)
            return OpenboxResultFactory.Failure(OpenboxErrorCode.ParameterError).ToActionResult();

        var payloadRequestObj = JsonSerializer.Deserialize(decryptedPayload, payloadType, OpenboxSerializer.Value);
        if (payloadRequestObj is null)
            return OpenboxResultFactory.Failure(OpenboxErrorCode.ParameterError).ToActionResult();

        var responseObj = await _mediator.Send(payloadRequestObj, cancellationToken);
        if (responseObj is not IOpenboxResult response)
            return OpenboxResultFactory.Failure(OpenboxErrorCode.InternalServiceError).ToActionResult();

        return response.ToActionResult();
    }
}