namespace Platipus.Wallet.Api.Controllers;

using System.Net.Mime;
using Abstract;
using Application.Requests.Wallets.Uis;
using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartupSettings.Filters;
using StartupSettings.Filters.Security;

[Route("wallet/uis")]
[MockedErrorActionFilter(Order = 1)]
[UisSecurityFilter(Order = 2)]
[Produces(MediaTypeNames.Application.Xml)]
[Consumes(MediaTypeNames.Application.Xml)]
public class WalletUisController : ApiController
{
    private readonly IMediator _mediator;

    public WalletUisController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("authenticate")]
    public async Task<IActionResult> Authenticate(
        [FromQuery] UisAuthenticateRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("change-balance")]
    public async Task<IActionResult> ChangeBalance(
        [FromQuery] UisChangeBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("status")]
    public async Task<IActionResult> Status(
        [FromQuery] UisStatusRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("get-balance")]
    public async Task<IActionResult> GetBalance(
        [FromQuery] UisGetBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("private/test/get-security-value")]
    public async Task<IActionResult> GetSecurityValue(
        string username,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Set<User>()
            .Where(c => c.Username == username)
            .Select(
                c => new
                {
                    UserId = c.Id,
                    Casino = new
                    {
                        ProviderId = c.Casino.InternalId,
                        c.Casino.SignatureKey,
                    }
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return ResultFactory.Failure(ErrorCode.UserNotFound).ToActionResult();

        var securityValue = "not implemented cuz not used";
        // = user.Map(u => UisSecurityHash.Compute(u.Casino.ProviderId, u.UserId, u.Casino.SignatureKey));

        return Ok(securityValue);
    }
}