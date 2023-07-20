﻿namespace Platipus.Wallet.Api.Application.Requests.Wallets.Anakatech;

using Base;
using FluentValidation;
using Helpers.Common;
using Newtonsoft.Json;
using Responses.Anakatech;
using Results.Anakatech.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record AnakatechGetPlayerBalanceRequest(
        [property: JsonProperty("secret")] string Secret,
        [property: JsonProperty("sessionId")] string SessionId,
        [property: JsonProperty("securityToken")] string SecurityToken,
        [property: JsonProperty("playerId")] string PlayerId,
        [property: JsonProperty("providerGameId")] string ProviderGameId,
        [property: JsonProperty("playMode")] int PlayMode)
    : IAnakatechRequest, IRequest<IAnakatechResult<AnakatechGetPlayerBalanceResponse>>
{
    public sealed class Handler
        : IRequestHandler<AnakatechGetPlayerBalanceRequest, IAnakatechResult<AnakatechGetPlayerBalanceResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IAnakatechResult<AnakatechGetPlayerBalanceResponse>> Handle(
            AnakatechGetPlayerBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.GetBalanceAsync(
                request.SessionId,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure || walletResult.Data is null)
            {
                return walletResult.ToAnakatechFailureResult<AnakatechGetPlayerBalanceResponse>();
            }

            var data = walletResult.Data;

            var response = new AnakatechGetPlayerBalanceResponse(
                true,
                MoneyHelper.ConvertToCents(data.Balance),
                data.Currency);

            return walletResult.ToAnakatechResult(response);
        }
    }

    public sealed class AnakatechGetPlayerBalanceRequestValidator : AbstractValidator<AnakatechGetPlayerBalanceRequest>
    {
        public AnakatechGetPlayerBalanceRequestValidator()
        {
            RuleFor(x => x.Secret)
               .NotEmpty()
               .Length(1, 24);

            RuleFor(x => x.SessionId)
               .NotEmpty()
               .Length(1, 256);

            RuleFor(x => x.SecurityToken)
               .NotEmpty()
               .Length(1, 256);

            RuleFor(x => x.PlayerId)
               .NotEmpty()
               .Length(1, 32);

            RuleFor(x => x.ProviderGameId)
               .NotEmpty()
               .Length(1, 256);

            RuleFor(x => x.PlayMode)
               .Must(x => x is 1 or 3)
               .WithMessage("PlayMode must be either RealMoney (1) or Anonymous (3)");
        }
    }
}