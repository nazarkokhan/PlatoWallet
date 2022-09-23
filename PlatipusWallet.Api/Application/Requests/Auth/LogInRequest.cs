namespace PlatipusWallet.Api.Application.Requests.Auth;

using Base.Requests;
using Base.Responses;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Options;
using Results.Common;
using Results.Common.Result;
using Results.Common.Result.WithData;

public record LogInRequest(
    string UserName,
    string Password,
    string CasinoId) : IRequest<IResult<BaseResponse>>
{
    public class Handler : IRequestHandler<LogInRequest, IResult<BaseResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<BaseResponse>> Handle(
            LogInRequest request,
            CancellationToken cancellationToken)
        {
            var casinoExist = await _context.Set<Casino>()
                .Where(c => c.Id == request.CasinoId)
                .AnyAsync(cancellationToken);

            if (!casinoExist)
                return ResultFactory.Failure<BaseResponse>(ErrorCode.InvalidCasinoId);

            var user = await _context.Set<User>()
                .Where(
                    u => u.UserName == request.UserName &&
                         u.CasinoId == request.CasinoId)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure<BaseResponse>(ErrorCode.InvalidUser);

            if (user.IsDisabled)
                return ResultFactory.Failure<BaseResponse>(ErrorCode.UserDisabled);
            
            if (user.Password != request.Password)
                return ResultFactory.Failure<BaseResponse>(ErrorCode.Unknown);
            
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