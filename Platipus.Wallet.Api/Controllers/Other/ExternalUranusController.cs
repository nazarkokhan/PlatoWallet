namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.Wallets.Evoplay.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.Security.Uranus;

[Route("external/uranus/")]
[ServiceFilter(typeof(UranusSecurityFilter), Order = 1)]
[JsonSettingsName(nameof(CasinoProvider.Uranus))]
[ProducesResponseType(typeof(UranusFailureResponse), StatusCodes.Status200OK)]
public sealed class ExternalUranusController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalUranusController(IMediator mediator) => 
        _mediator = mediator;
}