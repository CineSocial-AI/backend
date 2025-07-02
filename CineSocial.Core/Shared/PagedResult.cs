namespace CineSocial.Core.Shared;

public class PagedResult<T> : Result<IEnumerable<T>>
{
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    protected PagedResult(
        IEnumerable<T> data,
        int pageNumber,
        int pageSize,
        int totalCount)
        : base(true, data, string.Empty)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    protected PagedResult(string error) : base(false, default, error)
    {
        PageNumber = 0;
        PageSize = 0;
        TotalCount = 0;
        TotalPages = 0;
    }

    protected PagedResult(List<string> errors) : base(false, default, string.Empty, errors)
    {
        PageNumber = 0;
        PageSize = 0;
        TotalCount = 0;
        TotalPages = 0;
    }

    public static PagedResult<T> Success(
        IEnumerable<T> data,
        int pageNumber,
        int pageSize,
        int totalCount)
        => new(data, pageNumber, pageSize, totalCount);

    public static new PagedResult<T> Failure(string error) => new(error);

    public static new PagedResult<T> Failure(List<string> errors) => new(errors);
}