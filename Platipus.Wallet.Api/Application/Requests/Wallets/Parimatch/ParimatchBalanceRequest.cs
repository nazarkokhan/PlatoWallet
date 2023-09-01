namespace Platipus.Wallet.Api.Application.Requests.Wallets.Parimatch;

using Base;
using JetBrains.Annotations;
using Responses;
using Results.Parimatch;
using Results.Parimatch.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

[PublicAPI]
public sealed record ParimatchBalanceRequest(
        string Cid,
        string SessionToken)
    : IParimatchRequest, IRequest<IParimatchResult<ParimatchCommonResponse>>
{
    public sealed class Handler : IRequestHandler<ParimatchBalanceRequest, IParimatchResult<ParimatchCommonResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }


        public async Task<IParimatchResult<ParimatchCommonResponse>> Handle(
            ParimatchBalanceRequest request,
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
                data.Currency,
                data.Balance,
                data.Username,
                data.Username);

            return ParimatchResultFactory.Success(response);
        }
    }
}