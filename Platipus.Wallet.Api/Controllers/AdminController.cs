namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Admin;
using Application.Requests.Test;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("admin")]
public class AdminController : ApiController
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator) => _mediator = mediator;

    [HttpPost("error-mock")]
    public async Task<IActionResult> MockError(CreateErrorMockRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("casino")]
    public async Task<IActionResult> CreateCasino(CreateCasinoRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("casinos/page")]
    public async Task<IActionResult> GetCasinosPage([FromQuery] GetCasinosPageRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("users/page")]
    public async Task<IActionResult> GetUsersPage([FromQuery] GetUsersPageRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("award")]
    public async Task<IActionResult> CreateAward(CreateAwardRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPut("casinos/set-databet-provider")]
    public async Task<IActionResult> SetDatabetCasinoProvider(SetDatabetCasinoProviderRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}