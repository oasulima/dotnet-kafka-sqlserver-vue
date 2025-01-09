using LinqToDB.Data;
using Shared;

namespace Reporting.API.Data.Extensions;

public static class DataParametersExtensions
{
    public static List<DataParameter> AddAsJsonIfNotEmpty<T>(
        this List<DataParameter> parameters,
        string parameterName,
        IReadOnlyCollection<T>? values
    )
    {
        if (values == null || !values.Any())
        {
            return parameters;
        }

        parameters.Add(
            new DataParameter(parameterName, LinqToDB.DataType.VarChar)
            {
                Value = Converter.Serialize(values),
            }
        );

        return parameters;
    }
}
