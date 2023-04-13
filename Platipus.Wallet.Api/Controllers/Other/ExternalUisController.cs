namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.External.Psw;
using Extensions;
using Microsoft.AspNetCore.Mvc;

[Route("external/uis")]
public class ExternalUisController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalUisController(IMediator mediator) => _mediator = mediator;

    [HttpPost("award")]
    public async Task<IActionResult> CreateAward(UisCreateAwardRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}