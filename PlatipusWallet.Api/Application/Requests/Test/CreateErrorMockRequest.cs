namespace PlatipusWallet.Api.Application.Requests.Test;

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using Results.Common.Result;
using Domain.Entities;
using Infrastructure.Persistence;

public record CreateErrorMockRequest(
    Guid SessionId,
    string MethodPath,
    string Body,
    HttpStatusCode HttpStatusCode) : IRequest<IResult>
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
            var session = await _context.Set<Session>()
                .Where(e => e.Id == request.SessionId)
                .FirstOrDefaultAsync(cancellationToken);

            if (session is null)
                return ResultFactory.Failure(ErrorCode.BadParametersInTheRequest);

            if (session.ErrorMock is not null)
                return ResultFactory.Failure(ErrorCode.CouldNotTryToMockSessionError);

            var errorMock = new ErrorMock
            {
                MethodPath = request.MethodPath,
                Body = request.Body,
                HttpStatusCode = request.HttpStatusCode,
                SessionId = request.SessionId
            };

            _context.Add(errorMock);
            await _context.SaveChangesAsync(cancellationToken);

            return ResultFactory.Success();
        }
    }
}