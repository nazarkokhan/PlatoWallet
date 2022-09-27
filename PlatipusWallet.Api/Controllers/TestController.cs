namespace PlatipusWallet.Api.Controllers;

using Abstract;
using Application.Requests.Test;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("test")]
public class TestController : ApiController
{
    private readonly IMediator _mediator;

    public TestController(IMediator mediator) => _mediator = mediator;

    [HttpPost("error-mock")]
    public async Task<IActionResult> MockError(CreateErrorMockRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}