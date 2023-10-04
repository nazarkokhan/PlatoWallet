namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Services.DafabetGameApi.External;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("external/dafabet")]
[JsonSettingsName(WalletProvider.Dafabet)]
public sealed class ExternalDafabetController : ApiController
{
    private readonly IMediator _mediator;

    public ExternalDafabetController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("launch")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLaunchUrl(
        [FromBody] DafabetGetLaunchUrlRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}