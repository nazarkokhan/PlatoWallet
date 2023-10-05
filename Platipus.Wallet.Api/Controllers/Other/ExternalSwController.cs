namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.External.Sw;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("external/sw")]
[JsonSettingsName(WalletProvider.Sw)]
public sealed class ExternalSwController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalSwController(IMediator mediator) => _mediator = mediator;

    [HttpPost("freespin.do")]
    public async Task<IActionResult> CreateFreespin(
        [FromBody] SwCreateFreespinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("deletefreespin.do")]
    public async Task<IActionResult> DeleteFreespin(
        [FromBody] SwDeleteFreespinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("connect.do")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLaunchUrl(
        [FromBody] SwGetLaunchUrlRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}