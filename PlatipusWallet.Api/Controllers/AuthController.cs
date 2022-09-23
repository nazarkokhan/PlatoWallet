namespace PlatipusWallet.Api.Controllers;

using Abstract;
using Application.Requests.Auth;
using Application.Requests.Base.Requests;
using Application.Requests.Base.Responses;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("auth")]
public class AuthController : ApiController
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("signup")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> SignUp(SignUpRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("login")]
    [ProducesResponseType(typeof(LogInRequest.Response), StatusCodes.Status200OK)]
    public async Task<IActionResult> LogIn(LogInRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}