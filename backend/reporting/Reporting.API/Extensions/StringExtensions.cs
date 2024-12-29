using System;

namespace Reporting.API.Extensions;

public static class StringExtensions
{
    public static int? ToNullableInt(this string str)
    {
        if (int.TryParse(str, out var value))
        {
            return value;
        }

        return null;
    }

    public static decimal? ToNullableDecimal(this string str)
    {
        if (decimal.TryParse(str, out var value))
        {
            return value;
        }

        return null;
    }

    public static TimeOnly? ToNullableTimeOnly(this string str)
    {
        if (TimeOnly.TryParse(str, out var value))
        {
            return value;
        }

        return null;
    }
}