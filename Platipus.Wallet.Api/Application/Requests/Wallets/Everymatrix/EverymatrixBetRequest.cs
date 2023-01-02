namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Base.Response;
using Extensions;
using Results.Everymatrix;
using Results.Everymatrix.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;

public record EverymatrixBetRequest(
    string SupplierUser,
    string TransactionUuid,
    Guid Token,
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
    EverymatrixMetaDto? Meta) : IEverymatrixBaseRequest, IRequest<IEverymatrixResult<EverymatrixBalanceResponse>>
{
    public class Handler : IRequestHandler<EverymatrixBetRequest, IEverymatrixResult<EverymatrixBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IEverymatrixResult<EverymatrixBalanceResponse>> Handle(
            EverymatrixBetRequest request,
            CancellationToken cancellationToken)
        {
            var walletRequest = request.Map(
                r => new BetRequest(
                    r.Token,
                    r.SupplierUser,
                    r.Currency,
                    r.Round,
                    r.TransactionUuid,
                    r.RoundClosed,
                    r.Amount / 100000m));

            var walletResult = await _wallet.BetAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                return walletResult.ToEverymatrixResult<EverymatrixBalanceResponse>();

            var response = walletResult.Data.Map(
                d => new EverymatrixBalanceResponse(
                    (int)(d.Balance * 100000),
                    request.SupplierUser,
                    request.RequestUuid,
                    d.Currency));

            return EverymatrixResultFactory.Success(response);
        }
    }
}