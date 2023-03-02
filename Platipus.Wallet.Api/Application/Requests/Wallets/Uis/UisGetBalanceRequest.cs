namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis;

using System.Xml.Serialization;
using Base;
using Results.ResultToResultMappers;
using Results.Uis;
using Results.Uis.WithData;
using Services.Wallet;

#pragma warning disable CS8618
[XmlRoot("REQUEST")]
public record UisGetBalanceRequest : IUisUserIdRequest, IRequest<IUisResult<UisResponseContainer>>
{
    [XmlElement("USERID")]
    public string UserId { get; set; }

    [XmlElement("HASH")]
    public string? Hash { get; init; }

    public class Handler : IRequestHandler<UisGetBalanceRequest, IUisResult<UisResponseContainer>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IUisResult<UisResponseContainer>> Handle(
            UisGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.GetBalanceAsync(
                request.UserId,
                true,
                cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToUisResult<UisResponseContainer>();
            var data = walletResult.Data;

            var response = new UisGetBalanceResponse { Balance = data.Balance };

            var container = new UisResponseContainer
            {
                Request = request,
                Response = response
            };
            return UisResultFactory.Success(container);
        }
    }

    public string GetSource()
    {
        return UserId;
    }

    [XmlRoot("RESPONSE")]
    public record UisGetBalanceResponse
    {
        [XmlElement("RESULT")]
        public string Result { get; set; } = "OK";

        [XmlElement("BALANCE")]
        public decimal Balance { get; set; }
    }
}