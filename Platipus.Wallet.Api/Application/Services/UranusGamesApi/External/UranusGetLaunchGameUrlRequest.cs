﻿namespace Platipus.Wallet.Api.Application.Services.UranusGamesApi.External;

using Application.Requests.Wallets.Uranus.Base;
using Application.Requests.Wallets.Uranus.Data;
using Newtonsoft.Json;
using Requests;
using Results.ResultToResultMappers;
using Results.Uranus;
using Results.Uranus.WithData;
using Wallet;

public sealed record UranusGetLaunchGameUrlRequest(
        [property: JsonProperty("environment")] string Environment,
        [property: JsonProperty("apiRequest")] UranusGetLaunchUrlGameApiRequest ApiRequest)
    : IRequest<IUranusResult<UranusSuccessResponse<UranusGameUrlData>>>

{
    public sealed record Handler : IRequestHandler<UranusGetLaunchGameUrlRequest,
        IUranusResult<UranusSuccessResponse<UranusGameUrlData>>>
    {
        private readonly IWalletService _walletService;
        private readonly IUranusGameApiClient _gameApiClient;

        public Handler(
            IWalletService walletService,
            IUranusGameApiClient gameApiClient)
        {
            _walletService = walletService;
            _gameApiClient = gameApiClient;
        }

        public async Task<IUranusResult<UranusSuccessResponse<UranusGameUrlData>>> Handle(
            UranusGetLaunchGameUrlRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment,
                cancellationToken);

            if (walletResponse.Data is null)
            {
                return walletResponse.ToUranusFailureResult<UranusSuccessResponse<UranusGameUrlData>>();
            }

            var clientResponse = await _gameApiClient.GetGameLaunchUrlAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToUranusFailureResult<UranusSuccessResponse<UranusGameUrlData>>();

            var response =
                new UranusSuccessResponse<UranusGameUrlData>(new UranusGameUrlData(clientResponse.Data?.Data?.Data.Url!));

            return UranusResultFactory.Success(response);
        }
    }
}