namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Responses.Evenbet.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.NewFilterStyle;
using StartupSettings.Filters.Security.Evenbet;

[Route("wallet/evenbet/")]
[ServiceFilter(typeof(EvenbetMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(EvenbetSecurityFilter), Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.Evenbet))]
[ProducesResponseType(typeof(EvenbetFailureResponse), StatusCodes.Status200OK)]
public sealed class WalletEvenbetController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletEvenbetController(IMediator mediator)
    {
        _mediator = mediator;
    }
}