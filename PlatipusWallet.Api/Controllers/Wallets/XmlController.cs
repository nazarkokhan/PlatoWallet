namespace PlatipusWallet.Api.Controllers.Wallets;

using System.Net.Mime;
using Abstract;
using Application.Requests.Test;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// [Produces(MediaTypeNames.Application.Xml)]
// [Consumes(MediaTypeNames.Application.Xml)]
// public class XmlController : BaseApiController
// {
//     private readonly IMediator _mediator;
//
//     public XmlController(IMediator mediator) => _mediator = mediator;
//
//     [HttpPost("mock-error")]
//     public async Task<IActionResult> MockError(CreateErrorMockRequest request, CancellationToken cancellationToken)
//         => (await _mediator.Send(request, cancellationToken)).ToActionResult();
// }