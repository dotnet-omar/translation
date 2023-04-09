using Translation.Interfaces;

namespace Translation.Classes;

public class Pagination<T> : IPagination<T>
{
    public ICollection<T> Items { get; set; } = null!;
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
}