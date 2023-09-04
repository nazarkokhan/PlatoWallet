namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.Parimatch;
using Application.Requests.Wallets.Parimatch.Responses;
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
[ServiceFilter(typeof(ParimatchMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(ParimatchSecurityFilter), Order = 2)]
[JsonSettingsName(WalletProvider.Parimatch)]
[ProducesResponseType(typeof(ParimatchErrorResponse), StatusCodes.Status200OK)]
public sealed class WalletParimatchController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletParimatchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("playerInfo")]
    [ProducesResponseType(typeof(ParimatchPlayerInfoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> PlayerInfo(
        [PublicAPI, FromHeader(Name = ParimatchHeaders.XHubConsumer)] string sign,
        [FromBody] ParimatchPlayerInfoRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(ParimatchBetWinCancelResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        [PublicAPI, FromHeader(Name = ParimatchHeaders.XHubConsumer)] string sign,
        [FromBody] ParimatchBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("win")]
    [ProducesResponseType(typeof(ParimatchBetWinCancelResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        [PublicAPI, FromHeader(Name = ParimatchHeaders.XHubConsumer)] string sign,
        [FromBody] ParimatchWinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("cancel")]
    [ProducesResponseType(typeof(ParimatchBetWinCancelResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(
        [PublicAPI, FromHeader(Name = ParimatchHeaders.XHubConsumer)] string sign,
        [FromBody] ParimatchCancelRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("promoWin")]
    [ProducesResponseType(typeof(ParimatchBetWinCancelResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> PromoWin(
        [PublicAPI, FromHeader(Name = ParimatchHeaders.XHubConsumer)] string sign,
        [FromBody] ParimatchPromoWinRequest request,
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
           .Select(c => new { c.InternalId })
           .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return ResultFactory.Failure(ErrorCode.CasinoNotFound).ToActionResult();

        return Ok(casino.InternalId);
    }
}