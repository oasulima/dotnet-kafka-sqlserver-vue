namespace Admin.API.Models;

public class SelectValue<TValue>
{
    public TValue Value { get; set; }
    public string Label { get; set; }
}