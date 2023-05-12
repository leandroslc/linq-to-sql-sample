using System.Linq.Expressions;

namespace LinqToSql.Core;

public class DbQueryProvider : IQueryProvider
{
    public IQueryable CreateQuery(Expression expression)
    {
        var elementType = expression.Type.GetElementType()
            ?? throw new NotSupportedException();

        return (IQueryable)Activator.CreateInstance(
            typeof(DbCollection<>).MakeGenericType(elementType))!;
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new DbCollection<TElement>(this, expression);

    public string GetEvaluatedQuery(Expression expression)
    {
        var (query, _) = new DbQueryTranslator().Translate(expression);

        return query;
    }

    public object? Execute(Expression expression)
    {
        throw new NotImplementedException();
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return (TResult)this.Execute(expression)!;
    }
}
