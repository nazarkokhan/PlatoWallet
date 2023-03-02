namespace Platipus.Wallet.Api.Application.Requests.Test;

using System.ComponentModel;
using System.Net;
using System.Net.Mime;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record CreateErrorMockRequest(
    [property: DefaultValue("reevo_nazar")] string Username,
    MockedErrorMethod Method,
    [property: DefaultValue("your_json_or_xml")] string Body,
    [property: DefaultValue(200)] HttpStatusCode HttpStatusCode,
    [property: DefaultValue("json")] string ContentType,
    [property: DefaultValue(1)] int Count,
    [property: DefaultValue("00:00:10")] TimeSpan? Timeout) : IRequest<IPswResult>
{
    public class Handler : IRequestHandler<CreateErrorMockRequest, IPswResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IPswResult> Handle(
            CreateErrorMockRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .TagWith("GetUserWithMockedError")
                .Where(e => e.Username == request.Username)
                .Include(u => u.MockedErrors)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return PswResultFactory.Failure(PswErrorCode.BadParametersInTheRequest);

            if (user.MockedErrors.Any(m => m.Method == request.Method))
                return PswResultFactory.Failure(PswErrorCode.Duplication);

            var contentType = request.ContentType switch
            {
                "text" => MediaTypeNames.Text.Plain,
                "xml" => MediaTypeNames.Text.Xml,
                "html" => MediaTypeNames.Text.Html,
                "json" or _ => MediaTypeNames.Application.Json
            };

            if (request.Timeout > TimeSpan.FromMinutes(3))
                return PswResultFactory.Failure(PswErrorCode.MaxTimeout3Min);

            var mockedError = new MockedError
            {
                Method = request.Method,
                Body = request.Body,
                HttpStatusCode = request.HttpStatusCode,
                ContentType = contentType,
                Count = request.Count <= 0 ? 1 : request.Count,
                Timeout = request.Timeout,
                User = user
            };

            _context.Add(mockedError);
            await _context.SaveChangesAsync(cancellationToken);

            return PswResultFactory.Success();
        }
    }
}