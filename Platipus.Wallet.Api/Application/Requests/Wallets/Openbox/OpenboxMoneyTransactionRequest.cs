namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Base;
using Base.Response;
using Results.ResultToResultMappers;
using Services.Wallet;

public record OpenboxMoneyTransactionRequest(
    string Token,
    string GameUid,
    string GameCycleUid,
    string OrderUid,
    int OrderType,
    long OrderAmount) : IOpenboxBaseRequest, IRequest<IOpenboxResult<OpenboxBalanceResponse>>
{
    public class Handler : IRequestHandler<OpenboxMoneyTransactionRequest, IOpenboxResult<OpenboxBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IOpenboxResult<OpenboxBalanceResponse>> Handle(
            OpenboxMoneyTransactionRequest request,
            CancellationToken cancellationToken)
        {
            return request.OrderType switch
            {
                3 => await HandleBet(request, cancellationToken),
                4 => await HandleWin(request, cancellationToken),
                _ => OpenboxResultFactory.Failure<OpenboxBalanceResponse>(OpenboxErrorCode.ParameterError)
            };
        }

        private async Task<IOpenboxResult<OpenboxBalanceResponse>> HandleBet(
            OpenboxMoneyTransactionRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.BetAsync(
                request.Token,
                request.GameCycleUid,
                request.OrderUid,
                request.OrderAmount / 100m,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToOpenboxResult<OpenboxBalanceResponse>();
            var data = walletResult.Data;

            var response = new OpenboxBalanceResponse((long)(data.Balance * 100));

            return OpenboxResultFactory.Success(response);
        }

        private async Task<IOpenboxResult<OpenboxBalanceResponse>> HandleWin(
            OpenboxMoneyTransactionRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.WinAsync(
                request.Token,
                request.GameCycleUid,
                request.OrderUid,
                request.OrderAmount / 100m,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToOpenboxResult<OpenboxBalanceResponse>();
            var data = walletResult.Data;

            var response = new OpenboxBalanceResponse((long)(data.Balance * 100));

            return OpenboxResultFactory.Success(response);
        }
    }
}