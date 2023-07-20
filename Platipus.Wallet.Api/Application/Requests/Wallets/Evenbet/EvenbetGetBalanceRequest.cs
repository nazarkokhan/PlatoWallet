namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evenbet;

using Base;
using FluentValidation;
using Helpers.Common;
using Newtonsoft.Json;
using Responses.Evenbet;
using Results.Evenbet.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record EvenbetGetBalanceRequest([property: JsonProperty("token")] string Token)
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
                MoneyHelper.ConvertToCents(data!.Balance),
                DateTimeOffset.Now.ToUnixTimeSeconds().ToString());

            return walletResult.ToEvenbetResult(response);
        }
    }

    public sealed class EvenbetGetBalanceRequestValidator : AbstractValidator<EvenbetGetBalanceRequest>
    {
        public EvenbetGetBalanceRequestValidator()
        {
            RuleFor(x => x.Token)
               .NotEmpty()
               .WithMessage("Token is required!");
        }
    }
}