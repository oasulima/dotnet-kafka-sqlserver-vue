namespace Admin.API.Models;

public class SelectValue<TValue>
{
    public required TValue Value { get; set; }
    public required string Label { get; set; }
}
