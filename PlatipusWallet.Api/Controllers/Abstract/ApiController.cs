namespace PlatipusWallet.Api.Controllers.Abstract;

using Microsoft.AspNetCore.Mvc;
using PlatipusWallet.Api.Results.External;

[ApiController]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
public abstract class ApiController : Controller
{
}