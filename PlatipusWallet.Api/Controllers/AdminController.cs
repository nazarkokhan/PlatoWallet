namespace PlatipusWallet.Api.Controllers;

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

    [HttpPost("mock-error")]
    public async Task<IActionResult> MockError(CreateErrorMockRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("casino")]
    public async Task<IActionResult> CreateCasino(CreateCasinoRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("casino/get-page")]
    public async Task<IActionResult> GetCasinosPage(GetCasinosPageRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("casino/user/get-page")]
    public async Task<IActionResult> GetCasinoUsersPage(GetCasinoUsersPageRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("award")]
    public async Task<IActionResult> CreateAward(CreateAwardRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}