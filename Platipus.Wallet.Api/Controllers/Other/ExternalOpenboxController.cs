namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Services.OpenboxGameApi.Requests;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("external/openbox")]
[JsonSettingsName(WalletProvider.Openbox)]
public sealed class ExternalOpenboxController : ApiController
{
    private readonly IMediator _mediator;

    public ExternalOpenboxController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("launcher")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLaunchUrl(
        OpenboxGetLaunchUrlRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}