namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.Wallets.Evoplay.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.Security.Evoplay;

[Route("external/evoplay/")]
[ServiceFilter(typeof(EvoplaySecurityFilter), Order = 1)]
[JsonSettingsName(nameof(CasinoProvider.Evoplay))]
[ProducesResponseType(typeof(EvoplayFailureResponse), StatusCodes.Status200OK)]
public sealed class ExternalEvoplayController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalEvoplayController(IMediator mediator) => 
        _mediator = mediator;
}