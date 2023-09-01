namespace Platipus.Wallet.Api.Application.Requests.Wallets.Parimatch;

using Base;
using Helpers.Common;
using JetBrains.Annotations;
using Responses;
using Results.Parimatch;
using Results.Parimatch.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

[PublicAPI]
public sealed record ParimatchPlayerInfoRequest(
        string Cid,
        string SessionToken)
    : IRequest<IParimatchResult<ParimatchCommonResponse>>, IParimatchSessionRequest
{
    public sealed class Handler : IRequestHandler<ParimatchPlayerInfoRequest, IParimatchResult<ParimatchCommonResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }


        public async Task<IParimatchResult<ParimatchCommonResponse>> Handle(
            ParimatchPlayerInfoRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.GetBalanceAsync(
                request.SessionToken,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToParimatchFailureResult<ParimatchCommonResponse>();
            var data = walletResult.Data;

            var response = new ParimatchCommonResponse(
                data.Username,
                MoneyHelper.ConvertToCents(data.Balance),
                data.Currency,
                data.Username);

            return ParimatchResultFactory.Success(response);
        }
    }
}