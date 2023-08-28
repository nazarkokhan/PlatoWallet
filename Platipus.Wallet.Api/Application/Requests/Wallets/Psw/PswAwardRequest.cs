namespace Platipus.Wallet.Api.Application.Requests.Wallets.Psw;

using Base;
using Base.Response;
using FluentValidation;
using JetBrains.Annotations;
using Results.ResultToResultMappers;
using Services.Wallet;

[PublicAPI]
public record PswAwardRequest(
    string SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    decimal Amount,
    string AwardId,
    bool Finished) : IPswBaseRequest, IRequest<IPswResult<PswBalanceResponse>>
{
    public class Handler : IRequestHandler<PswAwardRequest, IPswResult<PswBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IPswResult<PswBalanceResponse>> Handle(
            PswAwardRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.AwardAsync(
                request.SessionId,
                request.RoundId,
                request.TransactionId,
                request.Amount,
                request.AwardId,
                request.Currency,
                request.Finished,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToPswResult<PswBalanceResponse>();
            var data = walletResult.Data;

            var response = new PswBalanceResponse(data.Balance);

            return PswResultFactory.Success(response);
        }
    }

    public class Validator : AbstractValidator<PswAwardRequest>
    {
        public Validator()
        {
            RuleFor(p => p.Amount)
                .PrecisionScale(28, 2, false);
        }
    }
}