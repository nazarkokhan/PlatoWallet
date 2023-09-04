namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.Synot;
using Application.Responses.Synot.Base;
using Application.Results.Synot;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.Constants.Synot;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.Security;

[Route("wallet/synot")]
// [ServiceFilter(typeof(SynotMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(SynotSecurityFilter), Order = 2)]
[JsonSettingsName(WalletProvider.Synot)]
public sealed class WalletSynotController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletSynotController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("session")]
    public async Task<IActionResult> Session(CancellationToken cancellationToken)
    {
        if (!HttpContext.Request.Headers.TryGetValue(SynotConstants.XEasToken, out var token))
        {
            return SynotResultFactory.Failure(SynotError.INVALID_TOKEN).ToActionResult();
        }

        var request = new SynotSessionRequest(token!);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    // [HttpGet("balance")]
    // public async Task<IActionResult> Balance(
    //     [FromBody] AnakatechGetPlayerBalanceRequest request,
    //     CancellationToken cancellationToken)
    //     => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    //
    // [HttpPost("bet")]
    // [ProducesResponseType(typeof(AnakatechDebitResponse), StatusCodes.Status200OK)]
    // public async Task<IActionResult> Debit(
    //     [FromBody] AnakatechDebitRequest request,
    //     CancellationToken cancellationToken)
    //     => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    //
    // [HttpPost("win")]
    // [ProducesResponseType(typeof(AnakatechCreditResponse), StatusCodes.Status200OK)]
    // public async Task<IActionResult> Credit(
    //     [FromBody] AnakatechCreditRequest request,
    //     CancellationToken cancellationToken)
    //     => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    //
    // [HttpPost("rollback")]
    // [ProducesResponseType(typeof(AnakatechRollbackResponse), StatusCodes.Status200OK)]
    // public async Task<IActionResult> Rollback(
    //     [FromBody] AnakatechRollbackRequest request,
    //     CancellationToken cancellationToken)
    //     => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}