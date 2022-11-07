namespace Platipus.Wallet.Api.Application.Requests.Base.Requests;

public abstract record OpenboxBasePayloadRequest;

public abstract record OpenboxBaseUniversalMethodRequest<T>(
    Guid Type,
    Guid Method,
    Guid VendorUid,
    DateTime Timestamp,
    T Payload) : BaseRequest
    where T : OpenboxBasePayloadRequest, IRequest<IResult>;
    
public abstract record OpenboxBaseUniversalMethodResponse<T>(
    Guid Type,
    Guid Method,
    Guid VendorUid,
    DateTime Timestamp,
    T Payload) : BaseRequest
    where T : OpenboxBasePayloadRequest, IRequest<IResult>;