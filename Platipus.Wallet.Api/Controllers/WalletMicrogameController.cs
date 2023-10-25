namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Responses.Microgame.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.Security;

[Route("wallet/microgame")]
//[ServiceFilter(typeof(MicrogameMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(MicrogameSecurityFilter), Order = 2)]
[JsonSettingsName(WalletProvider.Microgame)]
[ProducesResponseType(typeof(MicrogameErrorResponse), StatusCodes.Status400BadRequest)]
public sealed class WalletMicrogameController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletMicrogameController(IMediator mediator) => _mediator = mediator;
}