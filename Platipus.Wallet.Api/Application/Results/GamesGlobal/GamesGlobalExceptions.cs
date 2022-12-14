namespace Platipus.Wallet.Api.Application.Results.GamesGlobal;

using Horizon.XmlRpc.Core;

public static class GamesGlobalExceptions
{
    public static Exception ToException(this GamesGlobalErrorCode errorCode)
        => new XmlRpcFaultException((int)errorCode, errorCode.ToString());

    // public static Exception ToException(this GamesGlobalErrorCode errorCode)
    //     => errorCode switch
    //     {
    //         _ => Forbidden
    //     };
    //
    // public static readonly Exception Forbidden = new XmlRpcFaultException(401, "Auth failure");
}