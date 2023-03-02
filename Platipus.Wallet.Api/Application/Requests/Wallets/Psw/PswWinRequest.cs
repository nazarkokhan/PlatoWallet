namespace Platipus.Wallet.Api.Application.Requests.Wallets.Psw;

using Base;
using Base.Response;
using FluentValidation;
using Results.ResultToResultMappers;
using Services.Wallet;

public record PswWinRequest(
    string SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    bool Finished,
    decimal Amount) : IPswBaseRequest, IRequest<IPswResult<PswBalanceResponse>>
{
    public class Handler : IRequestHandler<PswWinRequest, IPswResult<PswBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IPswResult<PswBalanceResponse>> Handle(
            PswWinRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.WinAsync(
                request.SessionId,
                request.RoundId,
                request.TransactionId,
                request.Amount,
                request.Finished,
                request.Currency,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToPswResult<PswBalanceResponse>();
            var data = walletResult.Data;

            var response = new PswBalanceResponse(data.Balance);

            return PswResultFactory.Success(response);
        }
    }

    public class Validator : AbstractValidator<PswWinRequest>
    {
        public Validator()
        {
            RuleFor(p => p.Amount)
                .ScalePrecision(2, 28);
        }
    }
}