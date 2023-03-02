namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet;

using Base;
using Base.Response;
using Results.ResultToResultMappers;
using Services.Wallet;

public record DafabetGetBalanceRequest(
    string PlayerId,
    string Hash) : IDafabetRequest, IRequest<IDafabetResult<DafabetBalanceResponse>>
{
    public class Handler : IRequestHandler<DafabetGetBalanceRequest, IDafabetResult<DafabetBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IDafabetResult<DafabetBalanceResponse>> Handle(
            DafabetGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.GetBalanceAsync(request.PlayerId, true, cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToDafabetResult<DafabetBalanceResponse>();
            var data = walletResult.Data;

            var response = new DafabetBalanceResponse(data.Username, data.Currency, data.Balance);

            return DafabetResultFactory.Success(response);
        }
    }

    public string GetSource() => PlayerId;
}