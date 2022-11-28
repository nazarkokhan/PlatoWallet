namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.External;
using Application.Services.GamesApi.DTOs.Responses;
using Extensions;
using Microsoft.AspNetCore.Mvc;

[Route("external/psw")]
public class ExternalPswController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalPswController(IMediator mediator)
        => _mediator = mediator;

    [HttpGet("gamelist")]
    [ProducesResponseType(typeof(PswGetCasinoGamesListGamesApiResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> PswGames([FromQuery] GetPswCasinoGamesRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}