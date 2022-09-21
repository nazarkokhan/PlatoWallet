namespace PlatipusWallet.Api.Controllers;

using Abstract;
using Application.Requests.Admin;
using Application.Requests.Base;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("auth")]
public class AuthController : ApiController
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("login")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateUser(CreateUserRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}