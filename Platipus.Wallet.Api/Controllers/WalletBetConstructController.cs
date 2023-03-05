namespace Platipus.Wallet.Api.Controllers;

using System.Globalization;
using System.Text.Json;
using Abstract;
using Application.Requests.Wallets.BetConstruct;
using Application.Requests.Wallets.BetConstruct.Base.Response;
using Application.Requests.Wallets.Everymatrix.Base;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;
using StartupSettings.Filters.SkipFilterToGetHash;
using StartupSettings.Filters.TODO;
using BetConstructBaseResponse = Application.Requests.Wallets.BetConstruct.Base.Response.BetConstructBaseResponse;

[Route("wallet/betconstruct")]
[MockedErrorActionFilter(Order = 1)]
[BetConstructVerifyHashFilter(Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.BetConstruct))]
[ProducesResponseType(typeof(BetConstructErrorResponse), StatusCodes.Status200OK)]
public class WalletBetConstructController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletBetConstructController(IMediator mediator) => _mediator = mediator;


    [HttpPost("get-player-info")]
    [ProducesResponseType(typeof(BetConstructGetPlayerInfoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance(
        BetConstructGetPlayerInfoRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("withdraw")]
    [ProducesResponseType(typeof(BetConstructBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        BetConstructWithdrawRequest request,
        CancellationToken cancellationToken)
    => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("deposit")]
    [ProducesResponseType(typeof(BetConstructBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Win(
        BetConstructDepositRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("rollback")]
    [ProducesResponseType(typeof(BetConstructBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(
        BetConstructRollbackTransactionRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();


    [SkipVerifyFilter]
    [HttpPost("private/test/get-security-value")]
    public async Task<IActionResult> GetSecurityValue(
        string casinoId,
        DateTime time,
        BetConstructDataRequest data,
        [FromServices] WalletDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (data is null)
        {
            return ResultFactory.Failure(ErrorCode.Unknown).ToActionResult();
        }

        var casino = await dbContext.Set<Casino>()
            .Where(c => c.Id == casinoId)
            .Select(
                c => new
                {
                    c.SignatureKey,
                    SecuritySignKey = c.SignatureKey
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (casino is null)
            return ResultFactory.Failure(ErrorCode.CasinoNotFound).ToActionResult();

        var dataToCompare = JsonConvert.SerializeObject(data);

        var hash = BetConstructSecurityHash.Compute(
            time.ToString("dd-MM-yyyy HH:mm:ss"),
            dataToCompare,
            casino.SecuritySignKey);

        return Ok(hash);
    }
}