namespace Platipus.Wallet.Api.Controllers.Abstract;

using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Results.External;

[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)] //TODO
public abstract class ApiController : BaseApiController
{
}