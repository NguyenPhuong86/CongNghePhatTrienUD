namespace AppDemo.ViewModels;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public string? Keyword { get; set; }
    public string SortOrder { get; set; } = string.Empty;
    public int Page { get; set; }
    public int TotalPages { get; set; }
}
