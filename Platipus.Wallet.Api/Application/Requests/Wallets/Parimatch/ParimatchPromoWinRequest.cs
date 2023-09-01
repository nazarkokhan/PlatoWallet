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
public record ParimatchPromoWinRequest(
        string Cid,
        string PlayerId,
        string TxId,
        long Amount)
    : IRequest<IParimatchResult<ParimatchPromoWinResponse>>, IParimatchRequest
{
    public sealed class Handler : IRequestHandler<ParimatchPromoWinRequest, IParimatchResult<ParimatchPromoWinResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IParimatchResult<ParimatchPromoWinResponse>> Handle(
            ParimatchPromoWinRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.AwardAsync(
                request.PlayerId,
                request.TxId,
                request.TxId,
                MoneyHelper.ConvertFromCents(request.Amount),
                request.TxId,
                searchByUsername: true,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToParimatchFailureResult<ParimatchPromoWinResponse>();
            var data = walletResult.Data;

            var response = new ParimatchPromoWinResponse(
                request.TxId,
                DateTimeOffset.UtcNow, //TODO get from data
                MoneyHelper.ConvertToCents(data.Balance));

            return ParimatchResultFactory.Success(response);
        }
    }
}