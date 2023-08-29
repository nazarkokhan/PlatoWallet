namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.External.Everymatrix;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("external/everymatrix")]
[JsonSettingsName(WalletProvider.Everymatrix)]
public class ExternalEverymatrixController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalEverymatrixController(IMediator mediator) => _mediator = mediator;

    [HttpPost("awardbonus")]
    public async Task<IActionResult> CreateAward(
        [FromBody] EverymatrixCreateAwardRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("foreitbonus")]
    public async Task<IActionResult> DeleteAward(
        [FromBody] EverymatrixDeleteFreespinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}