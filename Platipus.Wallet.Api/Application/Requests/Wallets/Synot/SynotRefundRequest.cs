namespace Platipus.Wallet.Api.Application.Requests.Wallets.Synot;

using Base;
using Helpers.Common;
using Responses.Synot;
using Results.ResultToResultMappers;
using Results.Synot.WithData;
using Services.Wallet;

public sealed record SynotRefundRequest(
    long Id,
    long RoundId,
    long Amount,
    string? Token,
    bool Final) : ISynotOperationsRequest, IRequest<ISynotResult<SynotOperationsResponse>>
{
    public sealed class Handler : IRequestHandler<SynotRefundRequest, ISynotResult<SynotOperationsResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<ISynotResult<SynotOperationsResponse>> Handle(
            SynotRefundRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.RollbackAsync(
                request.Token!,
                roundId: request.RoundId.ToString(),
                transactionId: request.Id.ToString(),
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