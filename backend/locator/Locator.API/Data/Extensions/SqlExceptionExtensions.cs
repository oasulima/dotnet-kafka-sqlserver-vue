using Microsoft.Data.SqlClient;
using Locator.API.Data.Constants;

namespace Locator.API.Data.Extensions;

public static class SqlExceptionExtensions
{
    public static bool IsPrimaryKeyConstraintViolation(this SqlException sqlException)
    {
        return sqlException.Number == SqlErrorCodes.UniqueConstraintViolation
               && sqlException.Message.Contains("PRIMARY KEY");
    }

    public static bool IsUniqueKeyConstraintViolation(this SqlException sqlException)
    {
        return sqlException.Number == SqlErrorCodes.UniqueConstraintViolation
               && sqlException.Message.Contains("UNIQUE KEY");
    }
}
