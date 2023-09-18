namespace Platipus.Wallet.Api.Controllers;

using System.Net.Mime;
using System.Text.Json;
using Abstract;
using Application.Requests.Wallets.Vegangster;
using Application.Responses.Vegangster;
using Application.Responses.Vegangster.Base;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartupSettings;
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

[Route("wallet/private/vegangster")]
[JsonSettingsName(WalletProvider.Vegangster)]
[Produces(MediaTypeNames.Text.Plain)]
[Consumes(MediaTypeNames.Application.Json)]
public sealed class WalletVegangsterTestController : ApiController
{
    [HttpPost("get-security-value")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
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
                    PrivateGameServerSecuritySign = c.Params.VegangsterPrivateGameServerSecuritySign
                })
           .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return ResultFactory.Failure(ErrorCode.CasinoNotFound).ToActionResult();

        var rawRequestBytes = HttpContext.GetRequestBodyBytesItem();

        var securityValue = VegangsterSecuritySign.Compute(rawRequestBytes, casino.PrivateGameServerSecuritySign);

        return Ok(securityValue);
    }
}