namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.Vegangster;
using Application.Responses.Vegangster;
using Application.Responses.Vegangster.Base;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.NewFilterStyle;
using StartupSettings.Filters.Security;

[Route("wallet/vegangster")]
[ServiceFilter(typeof(VegangsterMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(VegangsterSecurityFilter), Order = 2)]
[JsonSettingsName(WalletProvider.Vegangster)]
[ProducesResponseType(typeof(VegangsterFailureResponse), StatusCodes.Status400BadRequest)]
public sealed class WalletVegangsterController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletVegangsterController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("player/balance")]
    [ProducesResponseType(typeof(VegangsterPlayerBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        [FromHeader(Name = VegangsterHeaders.XApiSignature)] string xApiSignature,
        VegangsterPlayerBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("transaction/bet")]
    [ProducesResponseType(typeof(VegangsterTransactionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        [FromHeader(Name = VegangsterHeaders.XApiSignature)] string xApiSignature,
        VegangsterBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("transaction/win")]
    [ProducesResponseType(typeof(VegangsterTransactionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        [FromHeader(Name = VegangsterHeaders.XApiSignature)] string xApiSignature,
        VegangsterWinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("transaction/rollback")]
    [ProducesResponseType(typeof(VegangsterTransactionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Rollback(
        [FromHeader(Name = VegangsterHeaders.XApiSignature)] string xApiSignature,
        VegangsterRollbackRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}