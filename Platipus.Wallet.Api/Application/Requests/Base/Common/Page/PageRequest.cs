namespace Platipus.Wallet.Api.Application.Requests.Base.Common.Page;

using System.ComponentModel;

public record PageRequest(
    [property: DefaultValue(10)] int Size,
    [property: DefaultValue(1)] int Number);