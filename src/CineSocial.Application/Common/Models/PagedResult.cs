namespace CineSocial.Application.Common.Models;

public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public PaginationMetadata Metadata { get; set; }

    public PagedResult(List<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        Metadata = new PaginationMetadata(pageNumber, pageSize, totalCount);
    }

    public static PagedResult<T> Create(List<T> items, int totalCount, int pageNumber, int pageSize)
    {
        return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }

    // Kolay Result'a çevirme için
    public Result<List<T>> ToResult(string? message = null)
    {
        return Result<List<T>>.SuccessPaged(Items, Metadata, message);
    }
}
