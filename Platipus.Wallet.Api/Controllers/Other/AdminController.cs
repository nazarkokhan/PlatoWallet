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

    [HttpPost("app-version")]
    public async Task<IActionResult> GetAppVersion(CancellationToken cancellationToken)
        => PswResultFactory.Success(App.Version).ToActionResult();

    [HttpPost("error-mock")]
    public async Task<IActionResult> MockError(CreateErrorMockRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpDelete("error-mock")]
    public async Task<IActionResult> MockError(DeleteErrorMockRequest request, CancellationToken cancellationToken)
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
        [FromQuery(Name = "")] GetUsersPageRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("award")]
    public async Task<IActionResult> CreateAward(CreateAwardRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPut("casinos/set-databet-provider")]
    public async Task<IActionResult> SetDatabetCasinoProvider(
        SetDatabetCasinoProviderRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}