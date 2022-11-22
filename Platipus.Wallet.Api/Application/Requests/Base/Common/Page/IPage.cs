namespace Platipus.Wallet.Api.Application.Requests.Base.Common.Page;

public interface IPage<out TItem>
{
    public IReadOnlyCollection<TItem> Items { get; }

    public long TotalCount { get; }
}