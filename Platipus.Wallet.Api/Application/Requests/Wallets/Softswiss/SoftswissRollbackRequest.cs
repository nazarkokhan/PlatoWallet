namespace Platipus.Wallet.Api.Application.Requests.Wallets.Softswiss;

using Base;
using Services.Wallet;

public record SoftswissRollbackRequest(
        string UserId,
        string Currency,
        string Game,
        string GameId,
        bool? Finished,
        IReadOnlyList<SoftswissRollbackRequest.RollbackAction> Actions)
    : ISoftswissBaseRequest, IRequest<ISoftswissResult<SoftswissRollbackRequest.Response>>
{
    public class Handler : IRequestHandler<SoftswissRollbackRequest, ISoftswissResult<Response>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<ISoftswissResult<Response>> Handle(
            SoftswissRollbackRequest request,
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
        IReadOnlyList<RollbackTransaction> Transactions);

    public record RollbackTransaction(
        string ActionId,
        string TxId,
        DateTime ProcessedAt);

    public record RollbackAction(
        string Action,
        string ActionId,
        string OriginalActionId);
}