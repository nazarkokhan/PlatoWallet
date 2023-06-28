namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.External.Psw;
using Application.Services.PswGamesApi.DTOs.Responses;
using Extensions;
using Microsoft.AspNetCore.Mvc;

[Route("external/psw")]
public class ExternalPswController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalPswController(IMediator mediator) => _mediator = mediator;

    [HttpPost("freebet/award")]
    public async Task<IActionResult> CreateAward(PswCreateAwardRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("game/list")]
    [ProducesResponseType(typeof(PswGetCasinoGamesListGamesApiResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> PswGames([FromQuery] GetPswCasinoGamesRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("game/buy")]
    [ProducesResponseType(typeof(PswGameBuyGamesApiResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GameBuy([FromBody] PswGameBuyRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}