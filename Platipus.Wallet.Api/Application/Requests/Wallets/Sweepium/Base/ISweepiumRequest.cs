using System.ComponentModel;
using Newtonsoft.Json;
using Platipus.Wallet.Api.Application.Requests.Base;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Base;

public interface ISweepiumRequest : IBaseWalletRequest
{
    public string Token { get; }
}