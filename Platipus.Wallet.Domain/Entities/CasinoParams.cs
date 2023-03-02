namespace Platipus.Wallet.Domain.Entities;

public static class CasinoParams
{
    public static string GetOpenboxVendorUid(this Casino casino) => (string)casino.Params[OpenboxVendorUid]!;
    public static string GetReevoCallerId(this Casino casino) => (string)casino.Params[ReevoCallerId]!;
    public static string GetReevoCallerPassword(this Casino casino) => (string)casino.Params[ReevoCallerPassword]!;

    public static string GetHub88PrivateWalletSecuritySign(this Casino casino)
        => (string)casino.Params[Hub88PrivateWalletSecuritySign]!;

    public static string GetHub88PublicGameServiceSecuritySign(this Casino casino)
        => (string)casino.Params[Hub88PublicGameServiceSecuritySign]!;

    public static string GetHub88PrivateGameServiceSecuritySign(this Casino casino)
        => (string)casino.Params[Hub88PrivateGameServiceSecuritySign]!;


    public const string OpenboxVendorUid = nameof(OpenboxVendorUid);

    public const string ReevoCallerId = nameof(ReevoCallerId);
    public const string ReevoCallerPassword = nameof(ReevoCallerPassword);

    public const string Hub88PrivateWalletSecuritySign = nameof(Hub88PrivateWalletSecuritySign);
    public const string Hub88PublicGameServiceSecuritySign = nameof(Hub88PublicGameServiceSecuritySign);
    public const string Hub88PrivateGameServiceSecuritySign = nameof(Hub88PrivateGameServiceSecuritySign);
}