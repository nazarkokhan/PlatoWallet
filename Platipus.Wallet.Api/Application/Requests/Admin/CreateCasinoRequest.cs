namespace Platipus.Wallet.Api.Application.Requests.Admin;

using System.ComponentModel;
using System.Text.Json.Nodes;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record CreateCasinoRequest(
    [property: DefaultValue("custom_psw")] string CasinoId,
    [property: DefaultValue("12345678")] string SignatureKey,
    CasinoProvider Provider,
    List<string> Currencies,
    Dictionary<string, JsonNode>? Params,
    [property: DefaultValue("test")] string? Environment) : IRequest<IResult>
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

            var environmentExist = await _context.Set<GameEnvironment>()
                .Where(e => e.Id == request.Environment)
                .AnyAsync(cancellationToken);

            if (!environmentExist)
                return ResultFactory.Failure(ErrorCode.EnvironmentDoesNotExists);

            var supportedCurrencies = await _context.Set<Currency>()
                .ToListAsync(cancellationToken);

            var matchedCurrencies = supportedCurrencies
                .Where(c => request.Currencies.Any(rc => rc == c.Id))
                .ToList();

            if (matchedCurrencies.Count != request.Currencies.Count)
                return ResultFactory.Failure(ErrorCode.InvalidCurrency);

            if (request.Provider is CasinoProvider.Dafabet)
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
                CasinoCurrencies = matchedCurrencies
                    .Select(c => new CasinoCurrencies { CurrencyId = c.Id })
                    .ToList(),
            };

            if (request.Params is not null)
                casino.Params = request.Params;

            if (casino.Provider is CasinoProvider.Reevo)
            {
                var callerId = (string?)casino.Params[CasinoParams.ReevoCallerId];
                var callerPassword = (string?)casino.Params[CasinoParams.ReevoCallerPassword];

                if (callerId is null || callerPassword is null)
                    return ResultFactory.Failure(ErrorCode.BadParametersInTheRequest);
            }

            if (casino.Provider is CasinoProvider.Openbox)
            {
                var vendorId = (string?)casino.Params[CasinoParams.OpenboxVendorUid];

                if (vendorId is null)
                    return ResultFactory.Failure(ErrorCode.BadParametersInTheRequest);
            }

            _context.Add(casino);
            await _context.SaveChangesAsync(cancellationToken);

            return ResultFactory.Success();
        }
    }
}