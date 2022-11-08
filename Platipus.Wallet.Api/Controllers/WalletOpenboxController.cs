namespace Platipus.Wallet.Api.Controllers;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Abstract;
using Application.Requests.Wallets.Openbox;
using Application.Requests.Wallets.Openbox.Base;
using Application.Requests.Wallets.Openbox.Base.Response;
using Application.Results.Openbox;
using Extensions;
using Extensions.SecuritySign;

[Route("wallet/openbox")]
public class WalletOpenboxController : ApiController
{
    private readonly IMediator _mediator;

    public WalletOpenboxController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(typeof(OpenboxSingleResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        OpenboxSingleRequest request,
        CancellationToken cancellationToken)
    {
        var decryptedPayload = OpenboxPayload.Decrypt(request.Payload, "1234567890123456");
        
        var payloadType = request.Method.ToString() switch
        {
           "1" => typeof(OpenboxBalanceRequest) 
        };

        var payloadRequestObj = JsonSerializer.Deserialize(decryptedPayload, payloadType);

        var basePayloadRequest = payloadRequestObj as OpenboxBaseRequest;

        basePayloadRequest!.Request = request;  
        var payloadRequest = basePayloadRequest as IRequest<IOpenboxResult>;
        
        var response = await _mediator.Send(payloadRequest!, cancellationToken);
        return response.ToActionResult();
    }
}