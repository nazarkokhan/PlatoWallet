namespace Platipus.Wallet.Api.Application.Requests.External;

using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Base.Responses;
using Domain.Entities;
using Options;
using Results.Common;
using Infrastructure.Persistence;
using Results.Common.Result.Factories;

public record SignUpRequest(
    string UserName,
    string Password,
    string CasinoId,
    string Currency,
    decimal Balance) : IRequest<IResult<BaseResponse>>
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
                .Include(u => u.Casino.CasinoCurrencies)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is not null)
                return ResultFactory.Failure<BaseResponse>(ErrorCode.Unknown);

            var casinoCurrency = await _context.Set<CasinoCurrencies>()
                .Where(c => c.CasinoId == request.CasinoId &&
                            c.Currency.Name == request.Currency)
                .Select(c => c.Currency)
                .FirstOrDefaultAsync(cancellationToken);

            if (casinoCurrency is null)
                return ResultFactory.Failure<BaseResponse>(ErrorCode.WrongCurrency);
            
            user = new User
            {
                UserName = request.UserName,
                Password = request.Password,
                Balance = request.Balance,
                CasinoId = request.CasinoId,
                CurrencyId = casinoCurrency.Id
            };

            _context.Add(user);

            await _context.SaveChangesAsync(cancellationToken);
            
            var result = new BalanceResponse(user.Balance);

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
            
            RuleFor(p => p.Balance)
                .ScalePrecision(2, 38);
        }
    }
}