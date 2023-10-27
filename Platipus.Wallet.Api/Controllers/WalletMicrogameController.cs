namespace Platipus.Wallet.Api.Controllers;

using System.ComponentModel.DataAnnotations;
using Abstract;
using Application.Requests.Wallets.Microgame;
using Application.Responses.Microgame;
using Application.Responses.Microgame.Base;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.NewFilterStyle;
using StartupSettings.Filters.Security;

[Route("wallet/microgame")]
[ServiceFilter(typeof(MicrogameMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(MicrogameSecurityFilter), Order = 2)]
[JsonSettingsName(WalletProvider.Microgame)]
[ProducesResponseType(typeof(MicrogameErrorResponse), StatusCodes.Status400BadRequest)]
public sealed class WalletMicrogameController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletMicrogameController(IMediator mediator) => _mediator = mediator;

    [HttpPost("Authenticate")]
    [ProducesResponseType(typeof(MicrogameAuthenticateResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Authenticate(
        [FromBody] [Required] MicrogameAuthenticateRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("Balance")]
    [ProducesResponseType(typeof(MicrogameGetBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance(
        [FromBody] [Required] MicrogameGetBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("Reserve")]
    [ProducesResponseType(typeof(MicrogameReserveResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        [FromBody] [Required] MicrogameReserveRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("Release")]
    [ProducesResponseType(typeof(MicrogameReleaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        [FromBody] [Required] MicrogameReleaseRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("CancelReserve")]
    [ProducesResponseType(typeof(MicrogameCancelReserveResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Rollback(
        [FromBody] [Required] MicrogameCancelReserveRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}