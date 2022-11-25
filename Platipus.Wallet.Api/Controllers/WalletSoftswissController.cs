namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.Hub88;
using Application.Requests.Wallets.Hub88.Base.Response;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;

[Route("wallet/softswiss")]
[MockedErrorActionFilter(Order = 1)]
[PswVerifySignatureFilter(Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.Softswiss))]
public class WalletSoftswissController : ApiController
{
    private readonly IMediator _mediator;

    public WalletSoftswissController(IMediator mediator)
        => _mediator = mediator;

    [HttpPost("user/balance")]
    [ProducesResponseType(typeof(Hub88BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        [FromHeader(Name = Hub88Headers.XHub88Signature)] string sign,
        Hub88GetBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("transaction/bet")]
    [ProducesResponseType(typeof(Hub88BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        [FromHeader(Name = Hub88Headers.XHub88Signature)] string sign,
        Hub88BetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("transaction/win")]
    [ProducesResponseType(typeof(Hub88BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        [FromHeader(Name = Hub88Headers.XHub88Signature)] string sign,
        Hub88WinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("transaction/rollback")]
    [ProducesResponseType(typeof(Hub88BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Rollback(
        [FromHeader(Name = Hub88Headers.XHub88Signature)] string sign,
        Hub88RollbackRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}