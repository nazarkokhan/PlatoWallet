namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.Admin;
using Extensions;
using Microsoft.AspNetCore.Mvc;

[Route("external/common")]
public class ExternalCommonController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalCommonController(IMediator mediator) => _mediator = mediator;

    [HttpPost("award")]
    public async Task<IActionResult> CreateAward(
        CreateAwardRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}