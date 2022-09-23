namespace PlatipusWallet.Api.Application.Requests.Test.Page;

public interface IPage<out TItem>
{
    public IReadOnlyCollection<TItem> Items { get; }

    public long TotalCount { get; }
}