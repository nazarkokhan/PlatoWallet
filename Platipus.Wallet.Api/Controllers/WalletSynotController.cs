namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.Synot;
using Application.Responses.Synot;
using Application.Results.Synot;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.Constants.Synot;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.NewFilterStyle;
using StartupSettings.Filters.Security;

[Route("wallet/synot")]
[ServiceFilter(typeof(SynotMockedErrorActionFilter), Order = 1)]
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
    [ProducesResponseType(typeof(SynotSessionResponse), StatusCodes.Status200OK)]
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

    [HttpGet("balance")]
    [ProducesResponseType(typeof(SynotGetBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(CancellationToken cancellationToken)
    {
        if (!HttpContext.Request.Headers.TryGetValue(SynotConstants.XEasToken, out var token))
        {
            return SynotResultFactory.Failure(SynotError.INVALID_TOKEN).ToActionResult();
        }

        var request = new SynotGetBalanceRequest(token!);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("bet")]
    [ProducesResponseType(typeof(SynotOperationsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        [FromBody] SynotBetRequest request,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.Request.Headers.TryGetValue(SynotConstants.XEasToken, out var token))
        {
            return SynotResultFactory.Failure(SynotError.INVALID_TOKEN).ToActionResult();
        }

        var requestToProcess = request with { Token = token! };
        var result = await _mediator.Send(requestToProcess, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("win")]
    [ProducesResponseType(typeof(SynotOperationsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Credit(
        [FromBody] SynotWinRequest request,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.Request.Headers.TryGetValue(SynotConstants.XEasToken, out var token))
        {
            return SynotResultFactory.Failure(SynotError.INVALID_TOKEN).ToActionResult();
        }

        var requestToProcess = request with { Token = token! };
        var result = await _mediator.Send(requestToProcess, cancellationToken);
        return result.ToActionResult();
    }
    
    [HttpPost("refund")]
    [ProducesResponseType(typeof(SynotOperationsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Refund(
        [FromBody] SynotRefundRequest request,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.Request.Headers.TryGetValue(SynotConstants.XEasToken, out var token))
        {
            return SynotResultFactory.Failure(SynotError.INVALID_TOKEN).ToActionResult();
        }

        var requestToProcess = request with { Token = token! };
        var result = await _mediator.Send(requestToProcess, cancellationToken);
        return result.ToActionResult();
    }
}