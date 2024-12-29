namespace Locator.API.Data.Exceptions;

public class UniqueConstraintViolationException : Exception
{
    public string ColumnName { get; set; }

    public UniqueConstraintViolationException(string columnName, Exception innerException) : base(innerException.Message, innerException)
    {
        ColumnName = columnName;
    }
}
