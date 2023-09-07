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
public record ParimatchBetRequest(
        string Cid,
        string SessionToken,
        string PlayerId,
        string ProductId,
        string TxId,
        string RoundId,
        bool RoundClosed,
        long Amount,
        string Currency)
    : IRequest<IParimatchResult<ParimatchBetWinCancelResponse>>, IParimatchSessionRequest
{
    public sealed class Handler : IRequestHandler<ParimatchBetRequest, IParimatchResult<ParimatchBetWinCancelResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IParimatchResult<ParimatchBetWinCancelResponse>> Handle(
            ParimatchBetRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.BetAsync(
                request.SessionToken,
                request.RoundId,
                request.TxId,
                MoneyHelper.ConvertFromCents(request.Amount),
                request.Currency,
                request.RoundClosed,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToParimatchFailureResult<ParimatchBetWinCancelResponse>();
            var data = walletResult.Data;

            var response = new ParimatchBetWinCancelResponse(
                data.Transaction.Id,
                data.Transaction.InternalId,
                data.Transaction.CreatedDate.ToUniversalTime(),
                MoneyHelper.ConvertToCents(data.Balance));

            return ParimatchResultFactory.Success(response);
        }
    }
}