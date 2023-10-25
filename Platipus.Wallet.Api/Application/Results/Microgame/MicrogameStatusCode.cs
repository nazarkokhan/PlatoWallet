namespace Platipus.Wallet.Api.Application.Results.Microgame;

using System.ComponentModel;

public enum MicrogameStatusCode
{
    [Description("Request successful.")]
    OK = 0,

    [Description("User was not logged in.")]
    NOUSER = 1,

    [Description("Internal server error.")]
    INTERNALERROR = 2,

    [Description("An unsupported currency was specified.")]
    INVALIDCURRENCY = 3,

    [Description("Wrong username or password.")]
    WRONGUSERNAMEPASSWORD = 4,

    [Description("Account is locked.")]
    ACCOUNTLOCKED = 5,

    [Description("Account is disabled.")]
    ACCOUNTDISABLED = 6,

    [Description("The requested amount is too high or too low.")]
    NOTENOUGHMONEY = 7,

    [Description("The system is unavailable for this request. Try again later.")]
    UNAVAILABLE = 8,

    [Description("The access token contained in the request is not valid.")]
    INVALIDACCESSTOKEN = 9,

    [Description("Generic Error.")]
    GENERICERROR = 10,

    [Description("The request has been just processed.")]
    DUPLICATETRANSACTION = 11
}