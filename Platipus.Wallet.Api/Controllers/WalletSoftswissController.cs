namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.Hub88.Base.Response;
using Application.Requests.Wallets.Softswiss;
using Application.Requests.Wallets.Softswiss.Base;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;
using StartupSettings.Filters.Security;

[Route("wallet/softswiss")]
[MockedErrorActionFilter(Order = 1)]
[SoftswissSecurityFilter(Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.Softswiss))]
[ProducesResponseType(typeof(SoftswissErrorResponse), StatusCodes.Status400BadRequest)]
public class WalletSoftswissController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletSoftswissController(IMediator mediator) => _mediator = mediator;

    [HttpPost("play")]
    [ProducesResponseType(typeof(Hub88BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Play(
        [FromHeader(Name = PswHeaders.XRequestSign)] string sign,
        SoftswissPlayRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("rollback")]
    [ProducesResponseType(typeof(Hub88BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Rollback(
        [FromHeader(Name = PswHeaders.XRequestSign)] string sign,
        SoftswissRollbackRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("freespins")]
    [ProducesResponseType(typeof(Hub88BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Freespins(
        [FromHeader(Name = PswHeaders.XRequestSign)] string sign,
        SoftswissFreespinsRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}