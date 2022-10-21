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

    [HttpPost("psw/get-hash-body")]
    public Task<IActionResult> PsvSignature(
        [FromBody] object request,
        [FromQuery(Name = "signature_key")] string signatureKey,
        CancellationToken cancellationToken)
        => Task.FromResult<IActionResult>(
            Ok(
                new
                {
                    Request = request,
                    SignatureKey = signatureKey
                }));

    [HttpPost("dafabet/get-hash-body")]
    public Task<IActionResult> DafabetSignature(
        [FromBody] Dictionary<string, string> request,
        [FromQuery] string method,
        [FromQuery(Name = "signature_key")] string signatureKey,
        CancellationToken cancellationToken)
    {
        // var items = request
        //     .OrderBy(x => x.Key)
        //     .Select(x => x.Value)
        //     .ToList();

        var source = string.Concat(request.Values);
        var result = new
        {
            Hash = DatabetHash.Compute($"{method}{source}", signatureKey)
        };

        return Task.FromResult<IActionResult>(Ok(result));
    }
}