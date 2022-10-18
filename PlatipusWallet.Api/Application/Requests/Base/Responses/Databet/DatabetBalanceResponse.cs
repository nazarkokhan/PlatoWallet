namespace PlatipusWallet.Api.Application.Requests.Base.Responses.Databet;

public record DatabetBalanceResponse(
    string PlayerId,
    string Currency,
    decimal Balance) : DatabetBaseResponse;