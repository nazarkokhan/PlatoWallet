namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus.Base;

using Newtonsoft.Json;

public record UranusCommonBoxResponse<TData> : IUranusBoxResponse<TData>
{
    protected UranusCommonBoxResponse()
    {
    }

    public UranusCommonBoxResponse(
        bool success,
        TData data,
        UranusCommonErrorResponse error)
    {
        Success = success;
        Data = data;
        Error = error;
    }

    [JsonProperty("success")]
    public bool Success { get; set; }
    
    [JsonProperty("data")]
    public TData Data { get; set; } = default!;

    [JsonProperty("error")]
    public UranusCommonErrorResponse Error { get; set; } = null!;
}