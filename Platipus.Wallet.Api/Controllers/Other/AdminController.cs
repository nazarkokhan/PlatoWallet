namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.Admin;
using Application.Requests.Test;
using Extensions;
using Microsoft.AspNetCore.Mvc;

[Route("admin")]
public class AdminController : RestApiController
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator) => _mediator = mediator;

    [HttpGet("a-why")]
    public IActionResult MockError() => Ok("ok");
    public async Task<IActionResult> MockError([FromQuery] GetErrorMocksRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("error-mock")]
    public async Task<IActionResult> MockError(CreateErrorMockRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpDelete("error-mock")]
    public async Task<IActionResult> MockError([FromQuery] DeleteErrorMockRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("environments")]
    public async Task<IActionResult> GetEnvironments(
        [FromQuery] GetGameEnvironmentsPageRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("environments")]
    public async Task<IActionResult> CreateEnvironment(
        CreateGameEnvironmentRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPut("environments")]
    public async Task<IActionResult> UpdateEnvironment(
        UpdateGameEnvironmentRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("casino")]
    public async Task<IActionResult> CreateCasino(CreateCasinoRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("casinos/page")]
    public async Task<IActionResult> GetCasinosPage(
        [FromQuery] GetCasinosPageRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("users/page")]
    public async Task<IActionResult> GetUsersPage(
        [FromQuery] GetUsersPageRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("award")]
    public async Task<IActionResult> CreateAward(CreateAwardRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}