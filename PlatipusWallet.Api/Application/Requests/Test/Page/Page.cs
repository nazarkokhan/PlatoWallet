namespace PlatipusWallet.Api.Application.Requests.Test.Page;

public record Page<T>(
    IReadOnlyCollection<T> Items,
    long TotalCount) : IPage<T>;