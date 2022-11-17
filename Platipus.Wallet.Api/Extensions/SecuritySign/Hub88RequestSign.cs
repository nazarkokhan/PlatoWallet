namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;

public static class Hub88RequestSign
{
    public static bool IsValidSign(string requestSign, byte[] rawRequestBody, string signatureKey)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(signatureKey);

        var isValid = rsa.VerifyData(
            rawRequestBody,
            Convert.FromBase64String(requestSign),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        return isValid;
    }

    public static string Compute(byte[] rawRequestBody, string signatureKey)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(signatureKey);

        var customEncrypted = rsa.SignData(rawRequestBody, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        var validSignature = Convert.ToBase64String(customEncrypted);

        return validSignature;
    }

    public const string KeyForWalletItself = @"-----BEGIN RSA PRIVATE KEY-----
MIICXAIBAAKBgQCnXQ7rU5lbYEGRSl4/PZVdAb9RTagu7k4M5m2sRg+x2c+vC6DY
VWTs3JWIcR8WxBYCcXIs8v7r10o55OOGxYTFLsZq28B8m3mVONm9jmXmrCRZQdjw
LLRt2wdTf3riMrdTbBs5BTWq3uq7eIx1wyTb2mVLhOnSeMqN2ug9ZzMsdwIDAQAB
AoGAGmco6mEm6W53CZfE5I8vmBuldgsdREIeGBTmKm5nHXSyOfhIqLhYErH9+Sd3
cxz+J+aDz6mKI+Sz9gwxBNr2Re/sc+L0pjZbtvC+BfF4E5kyfEMiZB/C6yPF6r78
nK83nNpQhtFe36Ka/MO4A2V2C4SMTaXAcFGnE0J5dVp1QEkCQQDXwHruO8tNRqoJ
TnUN0eHxbL+Ha5HVTlqUlbUvn+GQdkr5sR0lc12u6EsdU8o+Q3/N7Yblyyb5lcaT
Qa5fdMLtAkEAxpW6EDgtmoNPG7Yzfjm2/79p2cILHgCog9lHOzI+BQRLIQqtnhn6
jKxP87Pmlg2hzzbqR/XWgPqq3ahL1P6McwJBAML8TN1Totfy/ictBfL4dSd4rdwY
ZpMNniVc64PgDb4+emIRJJM9ITS68W/O0x/UZwPYICs7n7y/FNju6kWQw70CQDdR
XhNGku2HxnGhv5ZZ3XBAkevm8dHZvRd9XypORJiBKax6nQJ9mh21ok/wvTrvcTOX
yItjH+2t5gr45qbegaECQGHXR+M3KHTV1cxHIrmevs5CHylemSN2pS7WbovsdxHn
6zmKAQ7i6rk4P7ezL9/rtd6adFEWG6SYnlCUgv9d7dk=
-----END RSA PRIVATE KEY-----";

    public const string KeyForGameServer = @"-----BEGIN RSA PRIVATE KEY-----
MIICWwIBAAKBgHi57tRMYFBfHa8ZN5NTTSsK/iOKUBmOjhzZKrrZiLjraL/U9edz
ftNi5KaSoXOXLpiEOvaTD+fuuXGvDbME4+XBlfavFX8zza9FDLmERh9uhe+OLgwP
u4AebHvwt8uMY2Eg5+EOc4m0uvlEDI09U54WaxgNw9k4n3mnHboXXVHxAgMBAAEC
gYBtJg2bu4HIqHY5/N6WQFYgeEvU7hQFRzGNO3q6fDp0lcGaznuUyoL7swlu4FtA
GotyMPruO3/B/b+D3PTRybYQlmkLESdrffIwChTckIS29UkEtBAn5QE2ifOaa4Ic
KRACGi0gjOToulTS4HB2/0EWd5ilRjkqhWYvbFm2bHV6kQJBAOWtWZcHtG5BHveu
HWYBh26izHeMSztyTUKRTiC97Fl3/dYmSyEON4fDV9gNervSEvfOJgIpISmxwXeh
wL8sXlsCQQCGkAB5UvykV8aIR8k39f83jZT9e49YdGxXvg6WY8cylyQiO6Mw9mom
NAsqaeDSLbe9MpZroWvVRbvWnV5MGBqjAkEAmN48Vw3FxeyKFAhLgO1bmwO4W4mB
OVvmmHvmKFzAxvvac4KhVqsDwtT9zsuJ+SDlhxIqsh11+S5auqlqhNOfKQJAJiz6
hXEezf09DPLYynCXDIq1Z0jDvUOibS41c0Mxg0/P54pl3QE70kTXmhvZtadUxm9w
r25namVTSirxUsNP4wJAMCOSoOD1vOIVlRQ+qyZm9lEZOGUlSdcx4QLzepNYbEK7
8Dj8gIpH/b8QjPE2emdoYw1VDtUkWxj6dWu1wNLkBA==
-----END RSA PRIVATE KEY-----";
}