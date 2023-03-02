namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Base;
using Base.Response;
using Results.ResultToResultMappers;
using Services.Wallet;

public record OpenboxBalanceRequest(string Token) : IOpenboxBaseRequest, IRequest<IOpenboxResult<OpenboxBalanceResponse>>
{
    public class Handler : IRequestHandler<OpenboxBalanceRequest, IOpenboxResult<OpenboxBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IOpenboxResult<OpenboxBalanceResponse>> Handle(
            OpenboxBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.GetBalanceAsync(
                request.Token,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToOpenboxResult<OpenboxBalanceResponse>();
            var data = walletResult.Data;

            var response = new OpenboxBalanceResponse((long)(data.Balance * 100));

            return OpenboxResultFactory.Success(response);
        }
    }
}