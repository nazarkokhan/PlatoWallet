﻿namespace Platipus.Wallet.Api.Application.Services.UranusGamesApi.External;

using System.Text.Json.Serialization;
using Application.Requests.Wallets.Uranus.Base;
using Requests;
using Wallet;

public sealed record UranusCreateCampaignRequest(
        [property: JsonPropertyName("environment")] string Environment,
        [property: JsonPropertyName("apiRequest")] UranusCreateCampaignGameApiRequest ApiRequest)
    : IRequest<IResult<UranusSuccessResponse<string[]>>>
{
    public sealed class Handler : IRequestHandler<UranusCreateCampaignRequest, IResult<UranusSuccessResponse<string[]>>>
    {
        private readonly IWalletService _walletService;
        private readonly IUranusGameApiClient _uranusGameApiClient;

        public Handler(
            IWalletService walletService,
            IUranusGameApiClient uranusGameApiClient)
        {
            _walletService = walletService;
            _uranusGameApiClient = uranusGameApiClient;
        }

        public async Task<IResult<UranusSuccessResponse<string[]>>> Handle(
            UranusCreateCampaignRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment,
                cancellationToken);

            var clientResponse = await _uranusGameApiClient.CreateCampaignAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return ResultFactory.Failure<UranusSuccessResponse<string[]>>(
                    ErrorCode.GameServerApiError,
                    clientResponse.Exception);

            var response = new UranusSuccessResponse<string[]>(Array.Empty<string>());

            return ResultFactory.Success(response);
        }
    }
}