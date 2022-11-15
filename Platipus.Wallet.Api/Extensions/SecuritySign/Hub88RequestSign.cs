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

        return validSignature.ToLower();
    }

    public const string PrivateKey = @"-----BEGIN RSA PRIVATE KEY-----
MIIEowIBAAKCAQEAx3IRpSri/9SjA7f9me35v6LtJzn8drb1vg/UeGaPPFR16KsU
OPqbGJ2r1pRPJMedqqbO7Agt/HavWDcQhNZlc9VrVQcWK/w2HD9PflQYv0oQMiPK
5Mut/eIdFOpwwwaRAU4s6WOkJdSmP9F4cfr/amTZZoY59/t3SZWYjgZ9/LDI2X8S
3uLW/JPiH+6dm2bU8ykhxoWwLE/piJxynS73EzM0tgjHyTUMkarhK9qRVZk581/k
zmtJLLBZQl9XrQQcIfQ+zFZj7ijddOptxpxqCmq8gQNohB56p34yjVH3uAJAaFvI
Vg5mEkprrvNVDJwGonHSaaq7AICmzrF6h/r5dwIDAQABAoIBAFNIGIIlpGA7hE57
N9RdANq6x9iHaBqST48rwQb9nHYOtqWPOoSIcNcYj7ase1faWsX1nZYF3F39mT52
z9kIRZjW11jL+sAnMtkcvq77otHNtXGabJCZVHAdSROAydFGHqqy4CIcz2BUqY8g
gvDlZF4i+nzLM82PHcKGSwuTPmyTED37RqtscSxd7cGQkuL6OnohSFpW+5tvZcGZ
Ui4oVRrX4oVXz//3TDESRfondwQVKPoqQr7aiyYSKOJJMngIXmCvJ6p3XNiEW+dP
uXxm5N9QRkkX00v3vPTdsuwUjt3wepDJhR9BecRERRYNJIrqsgNxkJTJEDlsGqi5
kIKPb/ECgYEA8nKLPCfIaZ4G/7pUXQ64MMemS7qH17aU7Eb+mqDpFRt2bNr/yw12
qIhUkb6XyI/ZwOF2gdhPzte25CuZzNsMu3GqlEQ77AR1XCyu1AY8oiJS4TSmRovJ
x86BK8C8OY75myYcmRIvsusxviZfUCunDaexVGOqIKMRNrgJ2i8194UCgYEA0pgp
nzxI4H/Ej+5KOmEw9P2xstIXpW+CcEDbrh5pqW+PwNP/TUxC445Rn4R8AVXFVnbH
6DLrW35A9KBcA7Ve37vkVUILxPC+S967+gdRAd3BQTuUmi31+7cBjmNLKJMMC8Fl
DZ5S8zwRuVrU6o3bbPUbC5gDb/cN/7GVtb/D18sCgYEA2ma+8LCxxBr8GRAkATRK
Tn77WgqtZm/uVa5amrbLYR09IDBj7umw837kF+qGVsDnGu6/z5Ypxp3h/kccpELL
hGuPi0KwbBtUEXWbBBqeMjwTRxYjlzdDzP9Es0JLDNq0FcROTMHqQBXI2I8+mzzH
nvBqOSgS0JW04wMEtQyEY/UCgYBNMDKJR9paVtpf+vJABaGhGl+IcJL0MzP3Gv6q
CkGmNdrVzZ5U4a/eoipusmuVPa/P6keJZyh254a9Yw122oKEtOSTD1sq+yZ0vpXd
pdLeQT51P3ZPMKtpcIFkhCZnH8aZhHAalr5GouzIKG/D7OzROeGI1VXlMwNxhdCe
xkPtEwKBgG8IJGHMuzBGDtykqPwOdz7llWwzl3GgWiEDqt6kbIEr8F7CbbnlvrhJ
HRmd9tFEtYz2/3x9VTv9NCjd8FiWyPcivkd4dvMInfgBoCBJxw0I+nyjqQpiLuhx
x4xyX1tFKW+sAN+Mb9dO+ZHGpcz2FpPz5uXv6oCzTG9BLcgg0mwx
-----END RSA PRIVATE KEY-----";
}