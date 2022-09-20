namespace PlatipusWallet.Api.Filters;

using System.Security.Cryptography;
using System.Text;
using Domain.Entities;
using Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using Results.Common.Result;

public class SignatureValidationActionFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        
        var signPresent = httpContext.Request.Headers.TryGetValue("X-REQUEST-SIGN", out var signature);
        if(!signPresent)
        {
            context.Result = ResultFactory.Failure(ErrorCode.MissingSignature).ToActionResult();
            return;
        }
        
        var casinoIdPresent = context.ActionArguments.TryGetValue("casino_id", out var casinoIdObject);
        if (!casinoIdPresent || casinoIdObject is not string casinoId)
        {
            context.Result = ResultFactory.Failure(ErrorCode.BadParametersInTheRequest).ToActionResult();
            return;
        }

        var buffer = new byte[Convert.ToInt32(httpContext.Request.ContentLength)];
        _ = await httpContext.Request.Body.ReadAsync(buffer);
        
        var dbContext = httpContext.RequestServices.GetRequiredService<DbContext>();
        var casino = await dbContext.Set<Casino>()
            .Where(c => c.Id == casinoId)
            .FirstOrDefaultAsync();

        if (casino is null)
        {
            context.Result = ResultFactory.Failure(ErrorCode.InvalidCasinoId).ToActionResult();
            return;
        }
        
        var signatureBytes = Encoding.UTF8.GetBytes(casino.SignatureKey);
        var ownSignature = Convert.ToBase64String(HMACSHA256.HashData(signatureBytes, buffer));
        
        if (ownSignature != signature)
        {
            context.Result = ResultFactory.Failure(ErrorCode.InvalidSignature).ToActionResult();
        }

        await next();
    }
}