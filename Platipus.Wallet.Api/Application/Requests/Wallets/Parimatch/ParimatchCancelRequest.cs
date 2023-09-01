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
public record ParimatchCancelRequest(
        string Cid,
        string PlayerId,
        string ProductId,
        string TxId,
        string RoundId,
        long Amount,
        string Currency)
    : IRequest<IParimatchResult<ParimatchBetWinCancelResponse>>, IParimatchRequest
{
    public sealed class Handler : IRequestHandler<ParimatchWinRequest, IParimatchResult<ParimatchBetWinCancelResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }


        public async Task<IParimatchResult<ParimatchBetWinCancelResponse>> Handle(
            ParimatchWinRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.RollbackAsync(
                request.PlayerId,
                request.TxId,
                request.RoundId,
                searchByUsername: true,
                MoneyHelper.ConvertFromCents(request.Amount),
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToParimatchFailureResult<ParimatchBetWinCancelResponse>();
            var data = walletResult.Data;

            var response = new ParimatchBetWinCancelResponse(
                data.Transaction.Id,
                data.Transaction.InternalId,
                data.Transaction.CreatedDate,
                MoneyHelper.ConvertToCents(data.Balance));

            return ParimatchResultFactory.Success(response);
        }
    }
}