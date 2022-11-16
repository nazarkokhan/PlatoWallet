namespace Platipus.Wallet.Api.Application.Extensions;

using Requests.Base.Page;

public static class Extensions
{
    public static TResult Map<TSource, TResult>(this TSource source, Func<TSource, TResult> func)
        => func(source);

    public static IQueryable<TSource> SkipTake<TSource>(this IQueryable<TSource> source, PageRequest pagination)
    {
        var offset = (pagination.Number - 1) * pagination.Size;

        return source
            .Skip(offset)
            .Take(pagination.Size);
    }
}