using JetBrains.Annotations;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Psw;

using Base;
using Base.Response;
using FluentValidation;
using Results.ResultToResultMappers;
using Services.Wallet;

public record PswRollbackRequest(
    string SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    decimal Amount) : IPswBaseRequest, IRequest<IPswResult<PswBalanceResponse>>
{
    public class Handler : IRequestHandler<PswRollbackRequest, IPswResult<PswBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IPswResult<PswBalanceResponse>> Handle(
            PswRollbackRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.RollbackAsync(
                request.SessionId,
                request.TransactionId,
                request.RoundId,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToPswResult<PswBalanceResponse>();
            var data = walletResult.Data;

            var response = new PswBalanceResponse(data.Balance);

            return PswResultFactory.Success(response);
        }
    }

    [UsedImplicitly]
    public class Validator : AbstractValidator<PswRollbackRequest>
    {
        public Validator()
        {
            RuleFor(p => p.Amount)
                .PrecisionScale(28, 2, false);
        }
    }
}