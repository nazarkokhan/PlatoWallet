namespace Platipus.Wallet.Api.Application.Requests.External;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public sealed record CreateSessionRequest(
        [property: JsonPropertyName("username")] string Username,
        [property: DataType(DataType.Password)] string Password)
    : IRequest<IResult<string>>
{
    public sealed class Handler : IRequestHandler<CreateSessionRequest, IResult<string>>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext)
        {
            _walletDbContext = walletDbContext;
        }

        public async Task<IResult<string>> Handle(
            CreateSessionRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _walletDbContext.Set<User>()
               .Where(u => u.Username == request.Username)
               .SingleOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure<string>(ErrorCode.UserNotFound);

            if (user.IsDisabled)
                return ResultFactory.Failure<string>(ErrorCode.UserIsDisabled);

            if (!string.Equals(user.Password, request.Password, StringComparison.Ordinal))
                return ResultFactory.Failure<string>(ErrorCode.InvalidPassword);

            var session = new Session
            {
                UserId = user.Id,
                CreatedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(10),
                Id = Guid.NewGuid().ToString(),
                IsTemporaryToken = true,
                User = user
            };

            _walletDbContext.Add(session);

            await _walletDbContext.SaveChangesAsync(cancellationToken);

            return ResultFactory.Success(session.Id);
        }
    }

    public sealed class Validator : AbstractValidator<CreateSessionRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Username)
               .NotEmpty();

            RuleFor(x => x.Password).NotEmpty();
        }
    }
}