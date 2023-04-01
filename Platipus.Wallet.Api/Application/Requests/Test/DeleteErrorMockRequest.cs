namespace Platipus.Wallet.Api.Application.Requests.Test;

using System.ComponentModel;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record DeleteErrorMockRequest(
    [property: DefaultValue("reevo_platipus")] string Username,
    MockedErrorMethod Method) : IRequest<IResult>
{
    public class Handler : IRequestHandler<DeleteErrorMockRequest, IResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult> Handle(
            DeleteErrorMockRequest request,
            CancellationToken cancellationToken)
        {
            var userExists = await _context.Set<User>()
                .Where(u => u.Username == request.Username)
                .AnyAsync(cancellationToken);

            if (!userExists)
                return ResultFactory.Failure(ErrorCode.UserNotFound);

            var deletedErrorMocks = await _context.Set<MockedError>()
                .Where(e => e.User.Username == request.Username && e.Method == request.Method)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);

            if (deletedErrorMocks is 0)
                return ResultFactory.Failure(ErrorCode.BadParametersInTheRequest);

            return ResultFactory.Success();
        }
    }
}