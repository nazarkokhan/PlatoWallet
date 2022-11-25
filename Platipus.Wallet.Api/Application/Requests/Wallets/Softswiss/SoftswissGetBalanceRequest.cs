namespace Platipus.Wallet.Api.Application.Requests.Wallets.Softswiss;

using Base;
using Services.Wallet;
using Services.Wallet.DTOs;

public record SoftswissGetBalanceRequest(
    string UserId,
    string Currency,
    string Game,
    string? GameId,
    bool? Finished) : ISoftswissBaseRequest, IRequest<ISoftswissResult<GetBalanceRequest>>
{
    public class Handler : IRequestHandler<SoftswissGetBalanceRequest, ISoftswissResult<GetBalanceRequest>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<ISoftswissResult<GetBalanceRequest>> Handle(
            SoftswissGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            return null;
            // var walletRequest = request.Map(r => new GetBalanceRequest(r.SessionId, r.User));
            //
            // var walletResult = await _wallet.GetBalanceAsync(walletRequest, cancellationToken);
            // if (walletResult.IsFailure)
            //     walletResult.ToHub88Result();
            //
            // var response = walletResult.Data.Map(
            //     d => new Hub88BalanceResponse(
            //         (int)(d.Balance * 100000),
            //         request.SupplierUser,
            //         request.RequestUuid,
            //         d.Currency));
            //
            // return Hub88ResultFactory.Success(response);
        }
    }
}