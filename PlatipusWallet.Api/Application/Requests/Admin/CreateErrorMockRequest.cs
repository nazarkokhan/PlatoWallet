namespace PlatipusWallet.Api.Application.Requests.Admin;

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Base.Requests;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using Results.Common.Result;

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
            var errorMock = await _context.Set<ErrorMock>()
                .Where(e => e.SessionId == request.SessionId)
                .FirstOrDefaultAsync(cancellationToken);

            if (errorMock is null)
                return ResultFactory.Failure(ErrorCode.BadParametersInTheRequest);

            errorMock = new ErrorMock
            {
                MethodPath = request.MethodPath,
                Body = request.Body,
                ResponseStatusCode = request.HttpStatusCode,
                SessionId = request.SessionId
            };

            _context.Add(errorMock);
            await _context.SaveChangesAsync(cancellationToken);

            return ResultFactory.Success();
        }
    }
}