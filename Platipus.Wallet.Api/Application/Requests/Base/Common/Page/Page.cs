namespace Platipus.Wallet.Api.Application.Requests.Base.Common.Page;

public record Page<T>(
    IReadOnlyCollection<T> Items,
    long TotalCount) : IPage<T>;