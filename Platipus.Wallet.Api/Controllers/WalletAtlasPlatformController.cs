using Microsoft.AspNetCore.Mvc;
using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Base;
using Platipus.Wallet.Api.Controllers.Abstract;
using Platipus.Wallet.Api.StartupSettings.ControllerSpecificJsonOptions;
using Platipus.Wallet.Api.StartupSettings.Filters.Security;
using Platipus.Wallet.Domain.Entities.Enums;

namespace Platipus.Wallet.Api.Controllers;

[Route("wallet/atlas-platform/")]
//[ServiceFilter(typeof(AtlasPlatformMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(AtlasPlatformSecurityFilter), Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.AtlasPlatform))]
[ProducesResponseType(typeof(AtlasPlatformErrorResponse), StatusCodes.Status200OK)]
public sealed class WalletAtlasPlatformController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletAtlasPlatformController(IMediator mediator) => 
        _mediator = mediator;
    
    
}