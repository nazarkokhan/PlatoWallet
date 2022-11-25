namespace Platipus.Wallet.Api.Application.Requests.Wallets.Softswiss;

using Base;
using Services.Wallet;

public record SoftswissPlayRequest(
        string UserId,
        string Currency,
        string Game,
        string GameId,
        bool? Finished,
        IReadOnlyList<SoftswissPlayRequest.PlayAction> Actions)
    : ISoftswissBaseRequest, IRequest<ISoftswissResult<SoftswissPlayRequest.Response>>
{
    public class Handler : IRequestHandler<SoftswissPlayRequest, ISoftswissResult<Response>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<ISoftswissResult<Response>> Handle(
            SoftswissPlayRequest request,
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

    public record Response(
        int Balance,
        string GameId,
        IReadOnlyList<PlayTransaction> Transactions);

    public record PlayTransaction(
        string ActionId,
        string TxId,
        DateTime ProcessedAt,
        int? BonusAmount);

    public record PlayAction(
        string Action,
        int Amount,
        string ActionId,
        double? JackpotContribution,
        long? JackpotWin);
}