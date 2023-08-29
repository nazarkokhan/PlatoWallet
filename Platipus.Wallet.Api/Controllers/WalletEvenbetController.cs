namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.Evenbet;
using Application.Responses.Evenbet;
using Application.Responses.Evenbet.Base;
using Application.Services.EvenbetGameApi.External;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.NewFilterStyle;
using StartupSettings.Filters.Security.Evenbet;

[Route("wallet/evenbet/")]
[ServiceFilter(typeof(EvenbetMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(EvenbetSecurityFilter), Order = 2)]
[JsonSettingsName(WalletProvider.Evenbet)]
[ProducesResponseType(typeof(EvenbetFailureResponse), StatusCodes.Status200OK)]
public sealed class WalletEvenbetController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletEvenbetController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("balance")]
    [ProducesResponseType(typeof(EvenbetGetBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance(
        [FromBody] EvenbetGetBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("debit")]
    [ProducesResponseType(typeof(EvenbetDebitResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Debit(
        [FromBody] EvenbetDebitRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("credit")]
    [ProducesResponseType(typeof(EvenbetCreditResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Credit(
        [FromBody] EvenbetCreditRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("rollback")]
    [ProducesResponseType(typeof(EvenbetRollbackResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Rollback(
        [FromBody] EvenbetRollbackRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("login")]
    [ProducesResponseType(typeof(EvenbetLoginResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(
        [FromBody] EvenbetLoginRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}