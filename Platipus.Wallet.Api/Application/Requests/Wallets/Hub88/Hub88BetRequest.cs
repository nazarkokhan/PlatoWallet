namespace Platipus.Wallet.Api.Application.Requests.Wallets.Hub88;

using Base;
using Base.Response;
using Extensions;
using Results.Hub88;
using Results.Hub88.WithData;
using Results.Hub88.WithData.Mappers;
using Services.Wallet;
using Services.Wallet.DTOs;

public record Hub88BetRequest(
    string SupplierUser,
    string TransactionUuid,
    string Token,
    bool RoundClosed,
    string Round,
    string? RewardUuid,
    string RequestUuid,
    bool IsFree,
    int GameId,
    string GameCode,
    string Currency,
    string? Bet,
    int Amount,
    Hub88MetaDto? Meta) : Hub88BaseRequest(SupplierUser, Token, RequestUuid), IRequest<IHub88Result<Hub88BalanceResponse>>
{
    public class Handler : IRequestHandler<Hub88BetRequest, IHub88Result<Hub88BalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IHub88Result<Hub88BalanceResponse>> Handle(
            Hub88BetRequest request,
            CancellationToken cancellationToken)
        {
            var walletRequest = request.Map(
                r => new BetRequest(
                    new Guid(r.Token),
                    r.SupplierUser,
                    r.Currency,
                    r.GameCode,
                    r.Round,
                    r.TransactionUuid,
                    r.RoundClosed,
                    r.Amount / 100000m));

            var walletResult = await _wallet.BetAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                walletResult.ToHub88Result();

            var response = walletResult.Data.Map(
                d => new Hub88BalanceResponse(
                    (int) (d.Balance * 100000),
                    request.SupplierUser,
                    request.RequestUuid,
                    d.Currency));

            return Hub88ResultFactory.Success(response);
        }
    }
}