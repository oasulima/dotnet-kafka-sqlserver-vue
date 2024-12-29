namespace Locator.API.Utility;

public static class EnumerableExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> source)
    {
        return !source.Any();
    }
}
