namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay.Base;

using Newtonsoft.Json;

public record EvoplayCommonBoxResponse<TData> : IEvoplayBoxResponse<TData>
{
    protected EvoplayCommonBoxResponse()
    {
    }

    public EvoplayCommonBoxResponse(
        bool success,
        TData data,
        EvoplayCommonErrorResponse error)
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
    public EvoplayCommonErrorResponse Error { get; set; } = null!;
}