namespace Platipus.Wallet.Api.Controllers.Abstract;

using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public abstract class RestApiController : ApiController
{
}