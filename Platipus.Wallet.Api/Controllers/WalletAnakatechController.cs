namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Responses.Anakatech.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.Security;

[Route("wallet/anakatech/")]
//[ServiceFilter(typeof(AnakatechMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(AnakatechSecurityFilter), Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.Anakatech))]
[ProducesResponseType(typeof(AnakatechErrorResponse), StatusCodes.Status200OK)]
public sealed class WalletAnakatechController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletAnakatechController(IMediator mediator)
    {
        _mediator = mediator;
    }
}