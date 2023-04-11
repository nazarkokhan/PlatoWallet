namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.Admin;
using Application.Requests.External.Psw;
using Application.Services.PswGamesApi.DTOs.Responses;
using Extensions;
using Microsoft.AspNetCore.Mvc;

[Route("external/psw")]
public class ExternalPswController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalPswController(IMediator mediator)
        => _mediator = mediator;

    [HttpPost("award")]
    public async Task<IActionResult> CreateAward(PswCreateAwardRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("gamelist")]
    [ProducesResponseType(typeof(PswGetCasinoGamesListGamesApiResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> PswGames([FromQuery] GetPswCasinoGamesRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}