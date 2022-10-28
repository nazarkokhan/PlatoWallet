namespace Platipus.Wallet.Api.Results.Common.Pagination;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

public class Pagination<TData> : IPagination<TData>
    where TData : class
{
    public Pagination(int pageNumber, int pageSize, int totalItems)
    {
        PageSize = pageSize < 1 ? 1 : pageSize;
        TotalItems = totalItems;
        TotalPages = (int)Math.Ceiling((double)totalItems / PageSize);
        CurrentPage = pageNumber < 1 ? 1 : pageNumber <= TotalPages ? pageNumber : TotalPages;
        CurrentPageNumber = CurrentPage > 1 ? (CurrentPage - 1) * PageSize : 0;
        Items = Array.Empty<TData>();
    }

    public int PageSize { get; }
        
    public int TotalItems { get; }
        
    public int CurrentPage { get; }
        
    public int TotalPages { get; }

    [NotMapped]
    public int CurrentPageNumber { get; }

    public IList<TData> Items { get; private set; }

    public void SetItems(IList<TData> items) => Items = items;
}