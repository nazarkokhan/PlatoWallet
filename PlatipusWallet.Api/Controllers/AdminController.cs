namespace PlatipusWallet.Api.Controllers;

using Abstract;
using Application.Requests.Admin;
using Application.Requests.Base;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("admin")]
public class AdminController : ApiController
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator) => _mediator = mediator;

    [HttpPost("balance")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateUser(CreateUserRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}