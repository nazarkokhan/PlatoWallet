namespace Platipus.Wallet.Api.Application.Requests.Wallets.Synot;

using Base;
using Helpers.Common;
using Responses.Synot;
using Results.ResultToResultMappers;
using Results.Synot.WithData;
using Services.Wallet;

public sealed record SynotGetBalanceRequest(string? Token) : ISynotBaseRequest, IRequest<ISynotResult<SynotGetBalanceResponse>>
{
    public sealed class Handler : IRequestHandler<SynotGetBalanceRequest, ISynotResult<SynotGetBalanceResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<ISynotResult<SynotGetBalanceResponse>> Handle(
            SynotGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.GetBalanceAsync(
                request.Token!,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
            {
                return walletResult.ToSynotFailureResult<SynotGetBalanceResponse>();
            }

            var response = new SynotGetBalanceResponse(MoneyHelper.ConvertToCents(walletResult.Data.Balance));

            return walletResult.ToSynotResult(response);
        }
    }
}