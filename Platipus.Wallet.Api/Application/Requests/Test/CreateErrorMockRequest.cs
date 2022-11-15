namespace Platipus.Wallet.Api.Application.Requests.Test;

using System.Net;
using System.Net.Mime;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Psw;

public record CreateErrorMockRequest(
    string UserName,
    ErrorMockMethod Method,
    string Body,
    HttpStatusCode HttpStatusCode,
    string ContentType,
    int Count) : IRequest<IPswResult>
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

            var allowedMediaTypes = new List<string>
            {
                MediaTypeNames.Application.Json,
                MediaTypeNames.Text.Plain,
                MediaTypeNames.Text.Xml,
                MediaTypeNames.Text.Html
            };

            var mockedError = new MockedError
            {
                Method = request.Method,
                Body = request.Body,
                HttpStatusCode = request.HttpStatusCode,
                ContentType = allowedMediaTypes.Contains(request.ContentType)
                    ? request.ContentType
                    : MediaTypeNames.Application.Json,
                Count = request.Count <= 0 ? 1 : request.Count
            };

            user.MockedErrors.Add(mockedError);
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            return PswResultFactory.Success();
        }
    }
}