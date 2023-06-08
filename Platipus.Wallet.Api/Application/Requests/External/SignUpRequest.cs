namespace Platipus.Wallet.Api.Application.Requests.External;

using System.ComponentModel;
using Domain.Entities;
using DTO;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StartupSettings.Options;

public record SignUpRequest(
    [property: DefaultValue("psw_nazar_123")] string Username,
    [property: DefaultValue("qwe123")] string Password,
    [property: DefaultValue("psw")] string CasinoId,
    [property: DefaultValue("USD")] string Currency,
    [property: DefaultValue(1000)] decimal Balance) : IRequest<IResult>
{
    public class Handler : IRequestHandler<SignUpRequest, IResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult> Handle(
            SignUpRequest request,
            CancellationToken cancellationToken)
        {
            var casino = await _context.Set<Casino>()
                .Where(c => c.Id == request.CasinoId)
                .Select(
                    c => new
                    {
                        c.Id,
                        c.Provider
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (casino is null)
                return ResultFactory.Failure(ErrorCode.CasinoNotFound);

            var user = await _context.Set<User>()
                .Where(u => u.Username == request.Username && u.CasinoId == request.CasinoId)
                .Include(u => u.Casino.CasinoCurrencies)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is not null)
                return ResultFactory.Failure(ErrorCode.UserAlreadyExists);

            var casinoCurrency = await _context.Set<CasinoCurrencies>()
                .Where(c => c.CasinoId == request.CasinoId && c.Currency.Id == request.Currency)
                .Select(c => c.Currency)
                .FirstOrDefaultAsync(cancellationToken);

            if (casinoCurrency is null)
                return ResultFactory.Failure(ErrorCode.InvalidCurrency);

            user = new User
            {
                Username = request.Username,
                Password = request.Password,
                Balance = request.Balance,
                CasinoId = request.CasinoId,
                CurrencyId = casinoCurrency.Id,
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

            RuleFor(x => currenciesOptionsValue.Items.Contains(x.Currency));

            RuleFor(p => p.Password)
                .MinimumLength(6)
                .MaximumLength(20);

            RuleFor(p => p.Balance)
                .PrecisionScale(28, 2, false);
        }
    }
}