namespace PlatipusWallet.Api.Controllers;

using Abstract;
using Application.Requests.Admin;
using Application.Requests.Base.Requests;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("admin")]
public class AdminController : ApiController
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator) => _mediator = mediator;

    [HttpPost("mock-error")]
    // [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> MockError(CreateErrorMockRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}