namespace Platipus.Wallet.Api.Application.Requests.Base.Page;

public interface IPage<out TItem>
{
    public IReadOnlyCollection<TItem> Items { get; }

    public long TotalCount { get; }
}