namespace Platipus.Wallet.Api.Application.Requests.External;

using Domain.Entities;
using Domain.Entities.Enums;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StartupSettings.Options;
using Wallets.Psw.Base.Response;

public record SignUpRequest(
    string UserName,
    string Password,
    string CasinoId,
    string Currency,
    decimal Balance,
    int? SwUserId) : IRequest<IPswResult<PswBaseResponse>>
{
    public class Handler : IRequestHandler<SignUpRequest, IPswResult<PswBaseResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IPswResult<PswBaseResponse>> Handle(
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
                return PswResultFactory.Failure<PswBaseResponse>(PswErrorCode.InvalidCasinoId);

            var user = await _context.Set<User>()
                .Where(u => u.UserName == request.UserName && u.CasinoId == request.CasinoId)
                .Include(u => u.Casino.CasinoCurrencies)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is not null)
                return PswResultFactory.Failure<PswBaseResponse>(PswErrorCode.Unknown);

            var casinoCurrency = await _context.Set<CasinoCurrencies>()
                .Where(c => c.CasinoId == request.CasinoId && c.Currency.Name == request.Currency)
                .Select(c => c.Currency)
                .FirstOrDefaultAsync(cancellationToken);

            if (casinoCurrency is null)
                return PswResultFactory.Failure<PswBaseResponse>(PswErrorCode.WrongCurrency);

            int? swUserId = null;

            if (casino.Provider is CasinoProvider.Sw or CasinoProvider.GamesGlobal or CasinoProvider.Uis)
            {
                if (request.SwUserId is null)
                    return PswResultFactory.Failure<PswBaseResponse>(PswErrorCode.BadParametersInTheRequest);
                swUserId = request.SwUserId;
            }

            user = new User
            {
                UserName = request.UserName,
                Password = request.Password,
                Balance = request.Balance,
                CasinoId = request.CasinoId,
                CurrencyId = casinoCurrency.Id,
                SwUserId = swUserId
            };

            _context.Add(user);

            await _context.SaveChangesAsync(cancellationToken);

            var result = new PswBalanceResponse(user.Balance);

            return PswResultFactory.Success(result);
        }
    }

    public class Validator : AbstractValidator<SignUpRequest>
    {
        public Validator(IOptions<SupportedCurrenciesOptions> currenciesOptions)
        {
            var currenciesOptionsValue = currenciesOptions.Value;

            RuleFor(x => currenciesOptionsValue.Fiat.Contains(x.Currency) || currenciesOptionsValue.Crypto.Contains(x.Currency));

            RuleFor(p => p.Password)
                .MinimumLength(6)
                .MaximumLength(8);

            RuleFor(p => p.Balance)
                .ScalePrecision(2, 38);
        }
    }
}