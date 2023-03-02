namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Base;
using Base.Response;
using Results.ResultToResultMappers;
using Services.Wallet;

public record OpenboxCancelTransactionRequest(
    string Token,
    string GameUid,
    string GameCycleUid,
    string OrderUid,
    string OrderUidCancel) : IOpenboxBaseRequest, IRequest<IOpenboxResult<OpenboxBalanceResponse>>
{
    public class Handler : IRequestHandler<OpenboxCancelTransactionRequest, IOpenboxResult<OpenboxBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IOpenboxResult<OpenboxBalanceResponse>> Handle(
            OpenboxCancelTransactionRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.RollbackAsync(
                request.Token,
                request.OrderUid,
                request.GameCycleUid,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToOpenboxResult<OpenboxBalanceResponse>();
            var data = walletResult.Data;

            var response = new OpenboxBalanceResponse((long)(data.Balance * 100));

            return OpenboxResultFactory.Success(response);
        }
    }
}