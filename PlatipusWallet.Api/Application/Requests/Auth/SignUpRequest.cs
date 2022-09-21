namespace PlatipusWallet.Api.Application.Requests.Auth;

using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Options;
using Base;
using Results.Common.Result;
using Results.Common.Result.WithData;
using Responses;
using Results.Common;
using Results.External.Enums;

public record SignUpRequest(
    string UserName,
    string Password,
    string CasinoId,
    string Currency) : IRequest<IResult<BaseResponse>>
{
    public class Handler : IRequestHandler<SignUpRequest, IResult<BaseResponse>>
    {
        private readonly WalletDbContext _context;
        
        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<BaseResponse>> Handle(
            SignUpRequest request,
            CancellationToken cancellationToken)
        {
            var casinoExist = await _context.Set<Casino>()
                .Where(c => c.Id == request.CasinoId)
                .AnyAsync(cancellationToken);
            
            if(!casinoExist)
                return ResultFactory.Failure<BaseResponse>(ErrorCode.InvalidCasinoId);

            var user = await _context.Set<User>()
                .Where(u => u.UserName == request.UserName &&
                            u.CasinoId == request.CasinoId)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is not null)
                return ResultFactory.Failure<BaseResponse>(ErrorCode.Unknown);

            user = new User
            {
                UserName = request.UserName,
                Password = request.Password,
                Balance = 0,
                Currency = request.Currency,
            };

            _context.Add(user);

            await _context.SaveChangesAsync(cancellationToken);
            
            var result = new BalanceResponse(Status.Ok, user.Balance);

            return ResultFactory.Success(result);
        }
    }

    public class Validator : AbstractValidator<SignUpRequest>
    {
        public Validator(IOptions<SupportedCurrenciesOptions> currenciesOptions)
        {
            var currenciesOptionsValue = currenciesOptions.Value;

            RuleFor(x => currenciesOptionsValue.Fiat.Contains(x.Currency) ||
                         currenciesOptionsValue.Crypto.Contains(x.Currency));
            
            RuleFor(p => p.Password)
                .MinimumLength(6)
                .MaximumLength(8);
        }
    }
}