namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evenbet;

using Base;
using Newtonsoft.Json;
using Responses.Evenbet;
using Results.Evenbet.WithData;
using Services.Wallet;

public sealed record EvenbetGetBalanceRequest([property: JsonProperty("token")] string Token)
    : IEvenbetRequest, IRequest<IEvenbetResult<EvenbetGetBalanceResponse>>
{
    public sealed class Handler : IRequestHandler<EvenbetGetBalanceRequest, IEvenbetResult<EvenbetGetBalanceResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IEvenbetResult<EvenbetGetBalanceResponse>> Handle(
            EvenbetGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}