using System.Collections;

namespace PAW.Web.Models;

public interface IPagedList
{
    int Page { get; }
    int PageSize { get; }
    int TotalItems { get; }
    int TotalPages { get; }
}

/// <summary>
/// Simple in-memory page of items (max 25 per page by default).
/// </summary>
public class PagedList<T> : IPagedList, IEnumerable<T>
{
    public const int DefaultPageSize = 25;

    private readonly List<T> _items;

    private PagedList(List<T> items, int page, int pageSize, int totalItems, int totalPages)
    {
        _items = items;
        Page = page;
        PageSize = pageSize;
        TotalItems = totalItems;
        TotalPages = totalPages;
    }

    public int Page { get; }
    public int PageSize { get; }
    public int TotalItems { get; }
    public int TotalPages { get; }

    public static PagedList<T> Create(IEnumerable<T>? source, int page, int pageSize = DefaultPageSize)
    {
        var all = source?.ToList() ?? new List<T>();
        pageSize = Math.Clamp(pageSize, 1, DefaultPageSize);
        var totalPages = Math.Max(1, (int)Math.Ceiling(all.Count / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return new PagedList<T>(items, page, pageSize, all.Count, totalPages);
    }

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
