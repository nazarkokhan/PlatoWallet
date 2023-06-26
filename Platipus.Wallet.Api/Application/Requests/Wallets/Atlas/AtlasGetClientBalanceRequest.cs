namespace Platipus.Wallet.Api.Application.Requests.Wallets.Atlas;

using Base;
using Responses.AtlasPlatform;
using Platipus.Wallet.Api.Application.Services.Wallet;
using Results.Atlas;
using Results.Atlas.WithData;
using Results.ResultToResultMappers;

public sealed record AtlasGetClientBalanceRequest(
    string Token) : 
    IAtlasRequest, IRequest<IAtlasResult<AtlasCommonResponse>>
{
    public sealed class Handler :
        IRequestHandler<AtlasGetClientBalanceRequest,
            IAtlasResult<AtlasCommonResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<IAtlasResult<AtlasCommonResponse>> Handle(
            AtlasGetClientBalanceRequest request, CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.GetBalanceAsync(
                request.Token,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
            {
                walletResult.ToAtlasResult<AtlasCommonResponse>();
            }

            var data = walletResult.Data;
            var response = new AtlasCommonResponse(
                data.Currency, 
                (int)walletResult.Data.Balance, 
                data.UserId.ToString()
                );
            return AtlasResultFactory.Success(response);
        }
    }
}