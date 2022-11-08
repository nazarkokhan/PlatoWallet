namespace Platipus.Wallet.Api.Controllers;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Abstract;
using Application.Requests.Wallets.Openbox;
using Application.Requests.Wallets.Openbox.Base;
using Application.Requests.Wallets.Openbox.Base.Response;
using Application.Results.Openbox;
using Domain.Entities.Enums;
using Extensions;
using Extensions.SecuritySign;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("wallet/openbox")]
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
    private const string GetGameHistory = "";

    private readonly IMediator _mediator;

    public WalletOpenboxController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OpenboxSingleResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        OpenboxSingleRequest request,
        CancellationToken cancellationToken)
    {
        var decryptedPayload = OpenboxPayload.Decrypt(request.Payload, "1234567890123456");

        var payloadType = request.Method.ToString() switch
        {
            VerifyPlayer => typeof(OpenboxBalanceRequest),
            GetPlayerInformation => typeof(OpenboxBalanceRequest),
            GetPlayerBalance => typeof(OpenboxBalanceRequest),
            MoneyTransactions => typeof(OpenboxBalanceRequest),
            CancelTransaction => typeof(OpenboxBalanceRequest),
            Logout => typeof(OpenboxBalanceRequest),
            KeepTokenAlive => typeof(OpenboxBalanceRequest),
            GetGameHistory => typeof(OpenboxBalanceRequest),//TODO
            _ => null
        };

        if (payloadType is null)
            return OpenboxResultFactory.Failure(OpenboxErrorCode.ParameterError).ToActionResult();

        var payloadRequestObj = JsonSerializer.Deserialize(decryptedPayload, payloadType);

        var basePayloadRequest = payloadRequestObj as OpenboxBaseRequest;

        basePayloadRequest!.Request = request;
        var payloadRequest = basePayloadRequest as IRequest<IOpenboxResult>;

        var response = await _mediator.Send(payloadRequest!, cancellationToken);
        return response.ToActionResult();
    }
}