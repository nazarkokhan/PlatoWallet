namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.Base.Common;
using Application.Requests.DTOs;
using Application.Requests.External;
using Extensions;
using Microsoft.AspNetCore.Mvc;

[Route("external/games")]
public class ExternalGamelistController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalGamelistController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(List<GetCommonGameDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CommonErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CommonGames([FromQuery] GetCasinoGamesRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CommonErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGame(CreateGameRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CommonErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateGame(UpdateGameRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CommonErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteGame(DeleteGameRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("casino-gamelist")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CommonErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddCasinoGames(SetCasinoGamesRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}