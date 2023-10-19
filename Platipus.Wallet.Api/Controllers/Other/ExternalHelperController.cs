namespace Platipus.Wallet.Api.Controllers.Other;

using System.Net.Mime;
using Abstract;
using Application.Requests.External;
using Microsoft.AspNetCore.Mvc;

[Route("external/helper")]
[Produces(MediaTypeNames.Text.Plain)]
public sealed class ExternalHelperController : ApiController
{
    [HttpPost("unescape-launch")]
    public string Launch(LaunchRequest.Response request, CancellationToken cancellationToken)
    {
        return request.LaunchUrl;
    }
}