namespace Platipus.Wallet.Api.Results.Common.Pagination;

using System.Collections.Generic;

public interface IPagination<TData>
    where TData : class
{
    int PageSize { get; }

    int TotalItems { get; }

    int CurrentPage { get; }

    int TotalPages { get; }
        
    int CurrentPageNumber { get; }

    IList<TData> Items { get; }
}