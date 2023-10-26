namespace Platipus.Wallet.Api.Application.Requests.Admin;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Domain.Entities;
using Domain.Entities.Enums;
using FluentValidation;
using Infrastructure.Persistence;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

[PublicAPI]
public record CreateCasinoRequest(
    [property: DefaultValue("custom_psw")] string CasinoId,
    [property: DefaultValue("12345678")] string SignatureKey,
    WalletProvider Provider,
    List<string> Currencies,
    Casino.SpecificParams Params,
    [property: DefaultValue(new[] { "test" })] List<string> Environments) : IRequest<IResult>
{
    public class Handler : IRequestHandler<CreateCasinoRequest, IResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult> Handle(
            CreateCasinoRequest request,
            CancellationToken cancellationToken)
        {
            var casinoExist = await _context.Set<Casino>()
               .Where(e => e.Id == request.CasinoId)
               .AnyAsync(cancellationToken);

            if (casinoExist)
                return ResultFactory.Failure(ErrorCode.CasinoAlreadyExists);

            var environmentsExist = await _context.Set<GameEnvironment>()
               .Where(e => request.Environments.Contains(e.Id))
               .CountAsync(cancellationToken);

            if (environmentsExist != request.Environments.Count)
                return ResultFactory.Failure(ErrorCode.EnvironmentNotFound);

            var supportedCurrencies = await _context.Set<Currency>()
               .ToListAsync(cancellationToken);

            var matchedCurrencies = supportedCurrencies
               .Where(c => request.Currencies.Exists(rc => rc == c.Id))
               .ToList();

            if (matchedCurrencies.Count != request.Currencies.Count)
                return ResultFactory.Failure(ErrorCode.InvalidCurrency);

            if (request.Provider is WalletProvider.Dafabet)
            {
                var dafabetCasinoExist = await _context.Set<Casino>()
                   .Where(e => e.Provider == request.Provider)
                   .AnyAsync(cancellationToken);

                if (dafabetCasinoExist)
                    return ResultFactory.Failure(ErrorCode.ThisProviderSupportOnlyOneCasino);
            }

            var casino = new Casino(
                request.CasinoId,
                request.Provider,
                request.SignatureKey)
            {
                CasinoGameEnvironments = request.Environments
                   .Select(c => new CasinoGameEnvironments { GameEnvironmentId = c })
                   .ToList(),
                CasinoCurrencies = matchedCurrencies
                   .Select(c => new CasinoCurrencies { CurrencyId = c.Id })
                   .ToList(),
                Params = request.Params
            };

            if (!ValidateParams(casino))
                return ResultFactory.Failure(ErrorCode.BadParametersInTheRequest);

            _context.Add(casino);

            await _context.SaveChangesAsync(cancellationToken);
            return ResultFactory.Success();
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
        private static bool ValidateParams(Casino casino)
        {
            return casino.Provider switch
            {
                WalletProvider.SoftBet when casino.Params.ISoftBetProviderId is null => false,
                WalletProvider.Openbox when casino.Params.OpenboxVendorUid is null => false,
                WalletProvider.Hub88 when casino.Params.Hub88PrivateWalletSecuritySign is null
                                       || casino.Params.Hub88PublicGameServiceSecuritySign is null
                                       || casino.Params.Hub88PrivateGameServiceSecuritySign is null => false,
                WalletProvider.Reevo when casino.Params.ReevoCallerId is null || casino.Params.ReevoCallerPassword is null
                    => false,
                WalletProvider.EmaraPlay when casino.Params.EmaraPlayProvider is null => false,
                WalletProvider.Atlas when casino.Params.AtlasProvider is null => false,
                WalletProvider.Anakatech when casino.Params.AnakatechProvider is null => false,
                WalletProvider.Evenbet when casino.Params.EvenbetProvider is null => false,
                WalletProvider.Uranus when casino.Params.UranusProvider is null => false,
                WalletProvider.Vegangster when casino.Params.VegangsterProvider is null => false,
                WalletProvider.Microgame when casino.Params.MicrogameProvider is null => false,
                _ => true
            };
        }
    }

    public sealed class Validator : AbstractValidator<CreateCasinoRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Environments).NotEmpty();
        }
    }
}