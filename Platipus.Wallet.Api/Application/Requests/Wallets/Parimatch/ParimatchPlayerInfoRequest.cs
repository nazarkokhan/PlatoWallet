namespace Platipus.Wallet.Api.Application.Requests.Wallets.Parimatch;

using Base;
using Helpers;
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
    : IRequest<IParimatchResult<ParimatchPlayerInfoResponse>>, IParimatchSessionRequest
{
    public sealed class Handler : IRequestHandler<ParimatchPlayerInfoRequest, IParimatchResult<ParimatchPlayerInfoResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }


        public async Task<IParimatchResult<ParimatchPlayerInfoResponse>> Handle(
            ParimatchPlayerInfoRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.GetBalanceAsync(
                request.SessionToken,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToParimatchFailureResult<ParimatchPlayerInfoResponse>();
            var data = walletResult.Data;

            var response = new ParimatchPlayerInfoResponse(
                data.Username,
                MoneyHelper.ConvertToCents(data.Balance),
                data.Currency,
                data.Username);

            return ParimatchResultFactory.Success(response);
        }
    }
}