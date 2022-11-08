namespace Platipus.Wallet.Api.Controllers.Abstract;

using System.Net.Mime;
using Application.Requests.Wallets.Psw.Base.Response;
using Microsoft.AspNetCore.Mvc;

[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(PswErrorResponse), StatusCodes.Status400BadRequest)] //TODO
public abstract class ApiController : BaseApiController
{
}