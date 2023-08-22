namespace Platipus.Wallet.Api.Application.Requests.Wallets.Nemesis;

using Base;
using Infrastructure.Persistence;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Responses;
using Results.Nemesis;
using Results.Nemesis.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

[PublicAPI]
public sealed record NemesisBalanceRequest([property: JsonProperty("session_token")] string SessionToken)
    : INemesisRequest, IRequest<INemesisResult<NemesisBalanceResponse>>
{
    public sealed class Handler : IRequestHandler<NemesisBalanceRequest, INemesisResult<NemesisBalanceResponse>>
    {
        private readonly IWalletService _walletService;
        private readonly WalletDbContext _context;

        public Handler(IWalletService walletService, WalletDbContext context)
        {
            _walletService = walletService;
            _context = context;
        }

        public async Task<INemesisResult<NemesisBalanceResponse>> Handle(
            NemesisBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.GetBalanceAsync(
                request.SessionToken,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToNemesisFailureResult<NemesisBalanceResponse>();
            var data = walletResult.Data;

            var response = new NemesisBalanceResponse(
                NemesisMoneyHelper.FromBalance(data.Balance, data.Currency, out var multiplier),
                data.Currency,
                multiplier);

            return NemesisResultFactory.Success(response);
        }
    }
}