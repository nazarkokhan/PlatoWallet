namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.Admin;
using Extensions;
using Microsoft.AspNetCore.Mvc;

[Route("admin")]
public class AdminController : RestApiController
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator) => _mediator = mediator;

    [HttpGet("environment/page")]
    public async Task<IActionResult> GetEnvironments(
        [FromQuery] GetGameEnvironmentsPageRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("environment")]
    public async Task<IActionResult> CreateEnvironment(
        CreateGameEnvironmentRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPut("environment")]
    public async Task<IActionResult> UpdateEnvironment(
        UpdateGameEnvironmentRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("casino/page")]
    public async Task<IActionResult> GetCasinosPage(
        [FromQuery] GetCasinosPageRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("casino")]
    public async Task<IActionResult> CreateCasino(CreateCasinoRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("user/page")]
    public async Task<IActionResult> GetUsersPage(
        [FromQuery] GetUsersPageRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    //TODO no need for it?
    // [HttpPost("user/round")]
    // [ProducesResponseType(typeof(PswBalanceResponse), StatusCodes.Status200OK)]
    // public async Task<IActionResult> Round(AddRoundRequest request, CancellationToken cancellationToken)
    //     => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}