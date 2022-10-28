namespace Platipus.Wallet.Api.Application.Extensions;

using Requests.Base.Page;

public static class Extensions
{
    public static IQueryable<TSource> SkipTake<TSource>(this IQueryable<TSource> source, PageRequest pagination)
    {
        var offset = (pagination.Number - 1) * pagination.Size;

        return source
            .Skip(offset)
            .Take(pagination.Size);
    }
}