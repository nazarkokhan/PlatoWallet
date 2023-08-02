namespace Platipus.Wallet.Api.Controllers;

using System.Text.Json;
using Abstract;
using Application.Requests.Wallets.Hub88.Base.Response;
using Application.Requests.Wallets.Softswiss;
using Application.Requests.Wallets.Softswiss.Base;
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

[Route("wallet/softswiss")]
[ServiceFilter(typeof(SoftswissMockedErrorActionFilter), Order = 1)]
[SoftswissSecurityFilter(Order = 2)]
[JsonSettingsName(CasinoProvider.Softswiss)]
[ProducesResponseType(typeof(SoftswissErrorResponse), StatusCodes.Status400BadRequest)]
public class WalletSoftswissController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletSoftswissController(IMediator mediator) => _mediator = mediator;

    [HttpPost("play")]
    [ProducesResponseType(typeof(Hub88BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Play(
        [FromHeader(Name = PswHeaders.XRequestSign)] string sign,
        SoftswissPlayRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("rollback")]
    [ProducesResponseType(typeof(Hub88BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Rollback(
        [FromHeader(Name = PswHeaders.XRequestSign)] string sign,
        SoftswissRollbackRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("freespins")]
    [ProducesResponseType(typeof(Hub88BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Freespins(
        [FromHeader(Name = PswHeaders.XRequestSign)] string sign,
        SoftswissFreespinsRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}

[Route("wallet/private/softswiss")]
[JsonSettingsName(CasinoProvider.Softswiss)]
public class WalletSoftswissPrivateController : RestApiController
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
            .Select(c => new { c.SignatureKey })
            .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return ResultFactory.Failure(ErrorCode.UserNotFound).ToActionResult();

        var rawRequestBytes = HttpContext.GetRequestBodyBytesItem();

        var securityValue = SoftswissSecurityHash.Compute(rawRequestBytes, casino.SignatureKey);

        return Ok(securityValue);
    }
}