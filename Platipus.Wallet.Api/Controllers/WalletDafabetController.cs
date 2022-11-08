namespace Platipus.Wallet.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Application.Requests.Wallets.Dafabet;
using Application.Requests.Wallets.Dafabet.Base.Response;
using Abstract;
using Extensions;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;
using Domain.Entities.Enums;

[Route("wallet/dafabet")]
[ProducesResponseType(typeof(DatabetBaseResponse), StatusCodes.Status200OK)]
[MockedErrorActionFilter(Order = 1)]
[DatabetVerifySignatureFilter(Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.Dafabet))]
public class WalletDafabetController : ApiController
{
    private readonly IMediator _mediator;

    public WalletDafabetController(IMediator mediator) => _mediator = mediator;

    [HttpPost("authorize")]
    [ProducesResponseType(typeof(DatabetBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Authorize(
        DatabetAuthorizeRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("balance")]
    [ProducesResponseType(typeof(DatabetBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        DatabetGetBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(DatabetBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        DatabetBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("result")]
    [ProducesResponseType(typeof(DatabetBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> BetResult(
        DatabetBetResultRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bonusWin")]
    [ProducesResponseType(typeof(DatabetBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> BonusWin(
        DatabetBonusWinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("cancel")]
    [ProducesResponseType(typeof(DatabetBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CanselBet(
        DatabetCancelBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}