namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.Base.Common;
using Application.Requests.DTOs;
using Application.Requests.External;
using Extensions;
using Microsoft.AspNetCore.Mvc;

[Route("external/common")]
public class ExternalGamelistController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalGamelistController(IMediator mediator)
        => _mediator = mediator;

    [HttpGet("gamelist")]
    [ProducesResponseType(typeof(List<GetCommonGameDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CommonErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CommonGames([FromQuery] GetCasinoGamesRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("casino-gamelist")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CommonErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddCasinoGames(SetCasinoGamesRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}