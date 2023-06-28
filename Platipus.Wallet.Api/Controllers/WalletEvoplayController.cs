namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.Evoplay.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.NewFilterStyle;
using StartupSettings.Filters.Security.Evoplay;

[Route("wallet/evoplay/")]
[ServiceFilter(typeof(EvoplayMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(EvoplaySecurityFilter), Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.Evoplay))]
[ProducesResponseType(typeof(EvoplayCommonErrorResponse), StatusCodes.Status200OK)]
public sealed class WalletEvoplayController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletEvoplayController(IMediator mediator) => 
        _mediator = mediator;
    
    
}