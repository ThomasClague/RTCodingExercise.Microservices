namespace Contracts.Pagination;

public class PagedResponse<T>
{
    public Pagination Pagination { get; set; }

    public List<T> Data { get; set; }

    public PagedResponse(List<T> data, Pagination pagintaion)
    {
        Data = data;
        Pagination = pagintaion;
    }
}
