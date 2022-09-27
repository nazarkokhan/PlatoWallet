namespace PlatipusWallet.Api.Application.Requests.Base.Responses;

using PlatipusWallet.Api.Results.External.Enums;

public record BaseResponse(Status Status)
{
    // Ok by default in all responses
    // Status is stored in IResult and converted to error so
    protected BaseResponse() : this(Status.OK)
    {
    }
};