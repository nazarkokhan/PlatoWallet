namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.Nemesis;
using Application.Requests.Wallets.Nemesis.Responses;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions;
using Infrastructure.Persistence;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.NewFilterStyle;
using StartupSettings.Filters.Security;

[Route("wallet/parimatch")]
[ServiceFilter(typeof(NemesisMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(NemesisSecurityFilter), Order = 2)]
[JsonSettingsName(WalletProvider.Nemesis)]
[ProducesResponseType(typeof(NemesisErrorResponse), StatusCodes.Status400BadRequest)]
public sealed class WalletParimatchController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletParimatchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("balance")]
    [ProducesResponseType(typeof(NemesisBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        [PublicAPI, FromHeader(Name = NemesisHeaders.XIntegrationToken)] string sign,
        [FromBody] NemesisBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(NemesisBetWinResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        [PublicAPI, FromHeader(Name = NemesisHeaders.XIntegrationToken)] string sign,
        [FromBody] NemesisBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("win")]
    [ProducesResponseType(typeof(NemesisBetWinResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        [PublicAPI, FromHeader(Name = NemesisHeaders.XIntegrationToken)] string sign,
        [FromBody] NemesisWinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("cancel-transaction")]
    [ProducesResponseType(typeof(NemesisCancelTransactionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelTransaction(
        [PublicAPI, FromHeader(Name = NemesisHeaders.XIntegrationToken)] string sign,
        [FromBody] NemesisCancelTransactionRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}

[Route("wallet/private/parimatch")]
[JsonSettingsName(WalletProvider.Nemesis)]
public class WalletParimatchTestController : RestApiController
{
    [HttpPost("get-security-value")]
    public async Task<IActionResult> GetSecurityValue(
        string casinoId,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var casino = await dbContext.Set<Casino>()
           .Where(c => c.Id == casinoId)
           .Select(c => new { c.SignatureKey })
           .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return ResultFactory.Failure(ErrorCode.CasinoNotFound).ToActionResult();

        return Ok(casino.SignatureKey);
    }
}