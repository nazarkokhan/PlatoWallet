namespace Platipus.Wallet.Api.Application.Requests.External;

using System.Text.Json.Serialization;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public sealed record GetSessionByUserRequest([property: JsonPropertyName("userId")] int UserId)
    : IRequest<IResult<string>>
{
    public sealed class Handler : IRequestHandler<GetSessionByUserRequest, IResult<string>>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext)
        {
            _walletDbContext = walletDbContext;
        }

        public async Task<IResult<string>> Handle(
            GetSessionByUserRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _walletDbContext.Set<User>()
               .Where(s => s.Id == request.UserId)
               .SingleOrDefaultAsync(cancellationToken);

            if (user is null)
            {
                return ResultFactory.Failure<string>(ErrorCode.UserNotFound);
            }

            var session = await _walletDbContext.Set<Session>()
               .Where(s => s.UserId == user.Id)
               .Select(x => x.Id)
               .OrderBy(x => x)
               .LastOrDefaultAsync(cancellationToken);

            return string.IsNullOrWhiteSpace(session)
                ? ResultFactory.Failure<string>(ErrorCode.SessionNotFound)
                : ResultFactory.Success(session);
        }
    }

    public sealed class GetSessionByUserRequestValidator : AbstractValidator<GetSessionByUserRequest>
    {
        public GetSessionByUserRequestValidator()
        {
            RuleFor(x => x.UserId)
               .GreaterThan(0);
        }
    }
}