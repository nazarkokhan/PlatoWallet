namespace PlatipusWallet.Api.Controllers;

using Abstract;
using Application.Requests.Admin;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("test")]
public class TestController : ApiController
{
    private readonly IMediator _mediator;

    public TestController(IMediator mediator) => _mediator = mediator;

    [HttpGet("mock-error")]
    // [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> MockError(CreateErrorMockRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}