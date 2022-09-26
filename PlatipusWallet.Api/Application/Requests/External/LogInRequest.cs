namespace PlatipusWallet.Api.Application.Requests.External;

using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Base.Responses;
using Options;
using Results.Common;
using Results.Common.Result;
using Results.Common.Result.WithData;
using Domain.Entities;
using Infrastructure.Persistence;

public record LogInRequest(
    string UserName,
    string Password,
    string CasinoId) : IRequest<IResult<LogInRequest.Response>>
{
    public class Handler : IRequestHandler<LogInRequest, IResult<Response>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<Response>> Handle(
            LogInRequest request,
            CancellationToken cancellationToken)
        {
            var casinoExist = await _context.Set<Casino>()
                .Where(c => c.Id == request.CasinoId)
                .AnyAsync(cancellationToken);

            if (!casinoExist)
                return ResultFactory.Failure<Response>(ErrorCode.InvalidCasinoId);

            var user = await _context.Set<User>()
                .Where(
                    u => u.UserName == request.UserName &&
                         u.CasinoId == request.CasinoId)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure<Response>(ErrorCode.InvalidUser);

            if (user.IsDisabled)
                return ResultFactory.Failure<Response>(ErrorCode.UserDisabled);
            
            if (user.Password != request.Password)
                return ResultFactory.Failure<Response>(ErrorCode.Unknown);
            
            var session = new Session
            {
                ExpirationDate = DateTime.UtcNow.AddDays(1),
                User = user,
            };

            _context.Add(session);
            await _context.SaveChangesAsync(cancellationToken);

            var result = new Response(session.Id, user.Balance);

            return ResultFactory.Success(result);
        }
    }

    public record Response(
        Guid SessionId, 
        decimal Balance) : BalanceResponse(Balance);

    public class Validator : AbstractValidator<SignUpRequest>
    {
        public Validator(IOptions<SupportedCurrenciesOptions> currenciesOptions)
        {
            var currenciesOptionsValue = currenciesOptions.Value;

            RuleFor(
                x => currenciesOptionsValue.Fiat.Contains(x.Currency) ||
                     currenciesOptionsValue.Crypto.Contains(x.Currency));

            RuleFor(p => p.Password)
                .MinimumLength(6)
                .MaximumLength(8);
        }
    }
}