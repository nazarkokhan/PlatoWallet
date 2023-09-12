namespace Platipus.Wallet.Api.Application.Requests.Wallets.Synot;

using Base;
using Helpers;
using Models;
using Responses.Synot;
using Results.ResultToResultMappers;
using Results.Synot.WithData;
using Services.Wallet;

public sealed record SynotWinRequest(
    long Amount,
    long RoundId,
    long Id,
    bool Final,
    string? Token,
    SynotRoundDetail? RoundDetail,
    string? FreeGameOffer) : ISynotOperationsRequest, IRequest<ISynotResult<SynotOperationsResponse>>
{
    public sealed class Handler : IRequestHandler<SynotWinRequest, ISynotResult<SynotOperationsResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<ISynotResult<SynotOperationsResponse>> Handle(
            SynotWinRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.WinAsync(
                request.Token!,
                roundId: request.RoundId.ToString(),
                transactionId: request.Id.ToString(),
                roundFinished: request.Final,
                amount: MoneyHelper.ConvertFromCents(request.Amount),
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
            {
                return walletResult.ToSynotFailureResult<SynotOperationsResponse>();
            }

            var data = walletResult.Data;

            var response = new SynotOperationsResponse(
                long.Parse(data.Transaction.Id),
                MoneyHelper.ConvertToCents(data.Balance));

            return walletResult.ToSynotResult(response);
        }
    }
}