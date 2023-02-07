namespace Platipus.Wallet.Api.Application.Requests.Wallets.Reevo.Base;

public class ReevoMissingParameterException : Exception
{
    public ReevoMissingParameterException(string parameter)
        : base($"Missing {parameter} parameter")
    {
    }
}