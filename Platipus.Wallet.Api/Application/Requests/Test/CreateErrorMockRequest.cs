namespace Platipus.Wallet.Api.Application.Requests.Test;

using System.Net;
using System.Net.Mime;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record CreateErrorMockRequest(
    string UserName,
    ErrorMockMethod Method,
    string Body,
    HttpStatusCode HttpStatusCode,
    string ContentType,
    int Count,
    TimeSpan? Timeout) : IRequest<IPswResult>
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
                .Where(e => e.UserName == request.UserName)
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

            var mockedError = new MockedError
            {
                Method = request.Method,
                Body = request.Body,
                HttpStatusCode = request.HttpStatusCode,
                ContentType = contentType,
                Count = request.Count <= 0 ? 1 : request.Count,
                Timeout = request.Timeout
            };

            user.MockedErrors.Add(mockedError);
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            return PswResultFactory.Success();
        }
    }
}