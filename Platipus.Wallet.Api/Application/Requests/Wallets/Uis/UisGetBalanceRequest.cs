namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis;

using System.Xml.Serialization;
using Base;
using Base.Response;
using Results.ResultToResultMappers;
using Results.Uis;
using Results.Uis.WithData;
using Services.Wallet;

[XmlRoot("REQUEST")]
public record UisGetBalanceRequest
    : IUisUserIdRequest,
        IRequest<IUisResult<UisResponseContainer<UisGetBalanceRequest, UisGetBalanceRequest.UisGetBalanceResponse>>>
{
    [property: XmlElement("USERID")]
    public string UserId { get; set; } = null!;

    [property: XmlElement("HASH")]
    public string? Hash { get; set; }

    public class Handler
        : IRequestHandler<UisGetBalanceRequest, IUisResult<UisResponseContainer<UisGetBalanceRequest, UisGetBalanceResponse>>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IUisResult<UisResponseContainer<UisGetBalanceRequest, UisGetBalanceResponse>>> Handle(
            UisGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.GetBalanceAsync(
                request.UserId,
                true,
                cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToUisResult<UisResponseContainer<UisGetBalanceRequest, UisGetBalanceResponse>>();
            var data = walletResult.Data;

            var response = new UisGetBalanceResponse { Balance = data.Balance };

            var container = new UisResponseContainer<UisGetBalanceRequest, UisGetBalanceResponse>(request, response);

            return UisResultFactory.Success(container);
        }
    }

    public string GetSource()
    {
        return UserId;
    }

    public record UisGetBalanceResponse : UisBaseResponse
    {
        [XmlElement("BALANCE")]
        public decimal Balance { get; set; }
    }
}