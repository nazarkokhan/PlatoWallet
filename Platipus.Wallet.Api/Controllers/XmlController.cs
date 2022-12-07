namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Test;
using Extensions;
using Microsoft.AspNetCore.Mvc;

// [Produces(MediaTypeNames.Application.Xml)]
// [Consumes(MediaTypeNames.Application.Xml)]
public class XmlController : ApiController
{
    private readonly IMediator _mediator;

    public XmlController(IMediator mediator)
        => _mediator = mediator;

    [HttpPost("test")]
    public async Task<IActionResult> MockError(CreateErrorMockRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}