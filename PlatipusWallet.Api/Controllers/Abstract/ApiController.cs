namespace PlatipusWallet.Api.Controllers.Abstract;

using Microsoft.AspNetCore.Mvc;
using Results.External;

[ApiController]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
public abstract class ApiController : Controller
{
}