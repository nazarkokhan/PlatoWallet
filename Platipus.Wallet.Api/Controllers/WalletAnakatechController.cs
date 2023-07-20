namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.Anakatech;
using Application.Responses.Anakatech;
using Application.Responses.Anakatech.Base;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.NewFilterStyle;
using StartupSettings.Filters.Security;

[Route("wallet/anakatech/")]
[ServiceFilter(typeof(AnakatechMockedErrorActionFilter), Order = 1)]
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

    [HttpPost("balance")]
    [ProducesResponseType(typeof(AnakatechGetPlayerBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlayerBalance(
        [FromBody] AnakatechGetPlayerBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    // [HttpPost("debit")]
    // [ProducesResponseType(typeof(EvenbetDebitResponse), StatusCodes.Status200OK)]
    // public async Task<IActionResult> Debit(
    //     [FromBody] EvenbetDebitRequest request,
    //     CancellationToken cancellationToken)
    //     => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    //
    // [HttpPost("credit")]
    // [ProducesResponseType(typeof(EvenbetCreditResponse), StatusCodes.Status200OK)]
    // public async Task<IActionResult> Credit(
    //     [FromBody] EvenbetCreditRequest request,
    //     CancellationToken cancellationToken)
    //     => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    //
    // [HttpPost("rollback")]
    // [ProducesResponseType(typeof(EvenbetRollbackResponse), StatusCodes.Status200OK)]
    // public async Task<IActionResult> Rollback(
    //     [FromBody] EvenbetRollbackRequest request,
    //     CancellationToken cancellationToken)
    //     => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}