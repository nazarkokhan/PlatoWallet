namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.External;
using Application.Requests.Test;
using Application.Requests.Wallets.Psw.Base.Response;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.Attributes.Swagger;

[Route("external")]
public sealed class ExternalController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalController(IMediator mediator) => _mediator = mediator;

    [HttpPost("signup")]
    [ProducesResponseType(typeof(PswBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> SignUp(SignUpRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("session")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateSession(
        [FromBody] CreateSessionRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("session")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSessionByUser(
        [FromQuery] GetSessionByUserRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPut("session")]
    public async Task<IActionResult> UpdateSessionLifetime(
        [FromQuery] UpdateSessionLifetimeRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("launch")]
    [ProducesResponseType(typeof(LaunchRequest.Response), StatusCodes.Status200OK)]
    [ApplyRemoveNullProperties]
    public async Task<IActionResult> Launch(LaunchRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPut("balance")]
    [ProducesResponseType(typeof(LaunchRequest.Response), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddBalance(ChangeBalanceRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();


    [HttpGet("error-mock")]
    public async Task<IActionResult> MockError([FromQuery] GetErrorMocksRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("error-mock")]
    public async Task<IActionResult> MockError(CreateErrorMockRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpDelete("error-mock")]
    public async Task<IActionResult> MockError([FromQuery] DeleteErrorMockRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}