namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.External.Softswiss;
using Extensions;
using Microsoft.AspNetCore.Mvc;

[Route("external/softswiss")]
public class ExternalSoftswissController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalSoftswissController(IMediator mediator)
        => _mediator = mediator;

    [HttpPost("freespins/issue")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> IssueFreespins(
        ExternalSoftswissIssueFreespinsRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPut("freespins/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelFreespins(
        ExternalSoftswissCancelFreespinsRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("round/details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RoundDetails(
        [FromQuery] ExternalSoftswissRoundDetailsRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}