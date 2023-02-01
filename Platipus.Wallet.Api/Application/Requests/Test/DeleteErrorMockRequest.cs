namespace Platipus.Wallet.Api.Application.Requests.Test;

using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record DeleteErrorMockRequest(
    string UserName,
    ErrorMockMethod Method) : IRequest<IPswResult>
{
    public class Handler : IRequestHandler<DeleteErrorMockRequest, IPswResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IPswResult> Handle(
            DeleteErrorMockRequest request,
            CancellationToken cancellationToken)
        {
            var deletedErrorMocks = await _context.Set<MockedError>()
                .Where(e => e.User.UserName == request.UserName && e.Method == request.Method)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);

            if (deletedErrorMocks is 0)
                return PswResultFactory.Failure(PswErrorCode.BadParametersInTheRequest);

            return PswResultFactory.Success();
        }
    }
}