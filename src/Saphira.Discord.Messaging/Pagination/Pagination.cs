namespace Saphira.Discord.Messaging.Pagination;

public class Pagination
{
    public readonly int CurrentPage;
    public readonly int PageSize;
    public readonly int ItemCount;

    public Pagination(int currentPage, int pageSize, int itemCount)
    {
        CurrentPage = currentPage;
        PageSize = pageSize;
        ItemCount = itemCount;

        if (currentPage < 1)
        {
            throw new ArgumentException("The current page must be at least 1");
        }

        if (currentPage > GetPageCount())
        {
            throw new ArgumentException($"The current page can't be higher than {GetPageCount()}");
        }

        if (pageSize <= 0)
        {
            throw new ArgumentException("The page size must be more than 0");
        }
    }

    public int GetPreviousPage()
    {
        return CurrentPage == 1 ? 1 : CurrentPage - 1;
    }

    public int GetNextPage()
    {
        return IsLastPage() ? CurrentPage : CurrentPage + 1;
    }

    public bool IsFirstPage()
    {
        return CurrentPage == 1;
    }

    public bool IsLastPage()
    {
        return CurrentPage >= GetPageCount();
    }

    public double GetPageCount()
    {
        if (ItemCount == 0)
        {
            return 1;
        }

        return Math.Ceiling((double) ItemCount / PageSize);
    }

    public int GetOffset()
    {
        return (CurrentPage - 1) * PageSize;
    }

    public int GetLimit()
    {
        return PageSize;
    }
}
