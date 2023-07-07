namespace Platipus.Wallet.Api.Application.Requests.Wallets.Atlas;

using Base;
using Responses.AtlasPlatform;
using Platipus.Wallet.Api.Application.Services.Wallet;
using Results.Atlas;
using Results.Atlas.WithData;
using Results.ResultToResultMappers;

public sealed record AtlasBetRequest(
    string Token, 
    string RoundId, 
    decimal Amount,
    string TransactionId, 
    string Currency,
    string? BonusInstanceId = null) : 
        IRequest<IAtlasResult<AtlasCommonResponse>>, IAtlasRequest
{
    public sealed class Handler :
        IRequestHandler<AtlasBetRequest, IAtlasResult<AtlasCommonResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<IAtlasResult<AtlasCommonResponse>> Handle(
            AtlasBetRequest request, CancellationToken cancellationToken)
        {
            var validAmount = request.Amount / 100;
            var walletResult = await _walletService.BetAsync(
                request.Token,
                request.RoundId,
                request.TransactionId,
                amount: validAmount,
                currency: request.Currency,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToAtlasFailureResult<AtlasCommonResponse>();

            var data = walletResult.Data;
            var response = new AtlasCommonResponse(
                data?.Currency!,
                (long)(data!.Balance * 100), 
                data.UserId.ToString());

            return walletResult.ToAtlasResult(response);
        }
    }
}