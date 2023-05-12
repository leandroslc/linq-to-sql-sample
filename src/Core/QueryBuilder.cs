using System.Text;

namespace LinqToSql.Core;

public class QueryBuilder
{
    public string? TableName { get; set; }

    public ICollection<string> WhereExpressions { get; } = new List<string>();

    public Dictionary<string, object?> Parameters { get; } = new();

    public string AddParameter(object? value)
    {
        var paramName = $"p{Parameters.Count}";

        Parameters[paramName] = value;

        return paramName;
    }

    public string Build()
    {
        var builder = new StringBuilder($"SELECT * FROM {TableName}");

        if (WhereExpressions.Any())
        {
            builder.Append(" WHERE ").AppendJoin(" AND ", WhereExpressions);
        }

        if (Parameters.Any())
        {
            builder
                .Append(Environment.NewLine)
                .Append("-- (")
                .AppendJoin(", ", Parameters.Select(p => $"{p.Key}: '{p.Value}'"))
                .Append(")");
        }

        return builder.ToString();
    }
}
