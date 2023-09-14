// ReSharper disable UnusedParameter.Global

namespace Platipus.Wallet.Api.Controllers;

using System.Text.Json;
using Abstract;
using Application.Requests.Wallets.Hub88;
using Application.Requests.Wallets.Hub88.Base.Response;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartupSettings;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;
using StartupSettings.Filters.Security;

[Route("wallet/hub88")]
[MockedErrorActionFilter(Order = 1)]
[Hub88SecurityFilter(Order = 2)]
[JsonSettingsName(WalletProvider.Hub88)]
public class WalletHub88Controller : RestApiController
{
    private readonly IMediator _mediator;

    public WalletHub88Controller(IMediator mediator) => _mediator = mediator;

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

[Route("wallet/private/hub88")]
[JsonSettingsName(WalletProvider.Hub88)]
public class WalletHub88TestController : RestApiController
{
    [HttpPost("get-security-value")]
    public async Task<IActionResult> GetSecurityValue(
        string casinoId,
        [FromBody] JsonDocument request,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var casino = await dbContext.Set<Casino>()
           .Where(c => c.Id == casinoId)
           .Select(
                c => new
                {
                    c.SignatureKey,
                    PrivateWalletSecuritySign = c.Params.Hub88PrivateWalletSecuritySign
                })
           .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return ResultFactory.Failure(ErrorCode.CasinoNotFound).ToActionResult();

        var rawRequestBytes = HttpContext.GetRequestBodyBytesItem();

        var securityValue = Hub88SecuritySign.Compute(rawRequestBytes, casino.PrivateWalletSecuritySign);

        return Ok(securityValue);
    }
}