namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.Dafabet;
using Application.Requests.Wallets.Dafabet.Base.Response;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;
using StartupSettings.Filters.Security;

[Route("wallet/dafabet")]
[ProducesResponseType(typeof(DafabetBaseResponse), StatusCodes.Status200OK)]
[MockedErrorActionFilter(Order = 1)]
[DatabetSecurityFilter(Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.Dafabet))]
public class WalletDafabetController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletDafabetController(IMediator mediator) => _mediator = mediator;

    [HttpPost("authorize")]
    [ProducesResponseType(typeof(DafabetBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Authorize(
        DafabetAuthorizeRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("balance")]
    [ProducesResponseType(typeof(DafabetBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        DafabetGetBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(DafabetBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        DafabetBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("result")]
    [ProducesResponseType(typeof(DafabetBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> BetResult(
        DafabetBetResultRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bonusWin")]
    [ProducesResponseType(typeof(DafabetBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> BonusWin(
        DafabetBonusWinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("cancel")]
    [ProducesResponseType(typeof(DafabetBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CanselBet(
        DafabetCancelBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}