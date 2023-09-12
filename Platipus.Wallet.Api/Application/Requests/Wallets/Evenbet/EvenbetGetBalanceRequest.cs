namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evenbet;

using System.Text.Json.Serialization;
using Base;
using FluentValidation;
using Helpers;
using Responses.Evenbet;
using Results.Evenbet.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record EvenbetGetBalanceRequest([property: JsonPropertyName("token")] string Token)
    : IEvenbetRequest, IRequest<IEvenbetResult<EvenbetGetBalanceResponse>>
{
    public sealed class Handler : IRequestHandler<EvenbetGetBalanceRequest, IEvenbetResult<EvenbetGetBalanceResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IEvenbetResult<EvenbetGetBalanceResponse>> Handle(
            EvenbetGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.GetBalanceAsync(
                request.Token,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure || walletResult.Data is null)
            {
                return walletResult.ToEvenbetFailureResult<EvenbetGetBalanceResponse>();
            }

            var data = walletResult.Data;

            var response = new EvenbetGetBalanceResponse(
                (int)MoneyHelper.ConvertToCents(data!.Balance),
                DateTimeOffset.Now.ToUnixTimeSeconds().ToString());

            return walletResult.ToEvenbetResult(response);
        }
    }

    public sealed class EvenbetGetBalanceRequestValidator : AbstractValidator<EvenbetGetBalanceRequest>
    {
        public EvenbetGetBalanceRequestValidator()
        {
            RuleFor(x => x.Token) //TODO???
               .NotEmpty()
               .WithMessage("Token is required!");
        }
    }
}