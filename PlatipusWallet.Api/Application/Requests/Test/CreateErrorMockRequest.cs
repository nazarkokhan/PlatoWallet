namespace PlatipusWallet.Api.Application.Requests.Test;

using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using Results.Common.Result;
using Domain.Entities;
using Infrastructure.Persistence;
using Results.Common.Result.Factories;

public record CreateErrorMockRequest(
    string UserName,
    ErrorMockMethod Method,
    string Body,
    HttpStatusCode HttpStatusCode,
    string ContentType,
    int Count) : IRequest<IResult>
{
    public class Handler : IRequestHandler<CreateErrorMockRequest, IResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult> Handle(
            CreateErrorMockRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .TagWith("GetUserWithMockedError")
                .Where(e => e.UserName == request.UserName)
                .Include(u => u.MockedErrors)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure(ErrorCode.BadParametersInTheRequest);

            if (user.MockedErrors.Any(m => m.Method == request.Method))
                return ResultFactory.Failure(ErrorCode.Duplication);

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

            return ResultFactory.Success();
        }
    }
}