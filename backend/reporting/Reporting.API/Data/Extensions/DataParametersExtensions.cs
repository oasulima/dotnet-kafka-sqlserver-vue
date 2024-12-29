using LinqToDB.Data;
using Newtonsoft.Json;

namespace Reporting.API.Data.Extensions;

public static class DataParametersExtensions
{
    public static List<DataParameter> AddAsJsonIfNotEmpty<T>(this List<DataParameter> parameters,
        string parameterName, IReadOnlyCollection<T>? values)
    {
        if (values == null || !values.Any())
        {
            return parameters;
        }

        parameters.Add(new DataParameter(parameterName, LinqToDB.DataType.VarChar)
        {
            Value = JsonConvert.SerializeObject(values)
        });

        return parameters;
    }
}
