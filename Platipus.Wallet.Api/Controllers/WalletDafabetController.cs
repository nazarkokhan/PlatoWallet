namespace Platipus.Wallet.Api.Controllers;

using System.Text.Json;
using Abstract;
using Application.Requests.Wallets.Dafabet;
using Application.Requests.Wallets.Dafabet.Base.Response;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Platipus.Api.Common;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;
using StartupSettings.Filters.Security;

[Route("wallet/dafabet")]
[ProducesResponseType(typeof(DafabetBaseResponse), StatusCodes.Status200OK)]
[MockedErrorActionFilter(Order = 1)]
[DatabetSecurityFilter(Order = 2)]
[JsonSettingsName(WalletProvider.Dafabet)]
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

    [HttpPost("private/test/get-security-value")]
    public async Task<IActionResult> GetSecurityValue(
        [FromBody] Dictionary<string, JsonDocument> request,
        string casinoId,
        string method,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var casino = await dbContext.Set<Casino>()
            .Where(c => c.Id == casinoId && c.Provider == WalletProvider.Dafabet)
            .Select(c => new { c.SignatureKey })
            .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return ResultFactory.Failure(ErrorCode.CasinoNotFound).ToActionResult();

        var sourceValues = request.Values
            .Select(
                v => v.RootElement.Map(
                    r => r.ValueKind is JsonValueKind.True or JsonValueKind.False
                        ? r.GetString()!.ToLower()
                        : r.GetString()))
            .ToArray();

        var source = string.Concat(sourceValues);
        var securityValue = DatabetSecurityHash.Compute(method, source, casino.SignatureKey);

        return Ok(securityValue);
    }
}