namespace CineSocial.Application.Common.Models;

public class PaginationMetadata
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PaginationMetadata(int currentPage, int pageSize, int totalCount)
    {
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
