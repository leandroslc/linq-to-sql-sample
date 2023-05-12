using System.Collections;
using System.Linq.Expressions;

namespace LinqToSql.Core;

public class DbCollection<TElement>
		: IQueryable<TElement>, IOrderedQueryable<TElement>
{
    private DbQueryProvider provider;

    public DbCollection(DbQueryProvider provider)
    {
        this.provider = provider;
        Expression = Expression.Constant(this);
    }

    public DbCollection(DbQueryProvider provider, Expression expression)
    {
        this.provider = provider;
        Expression = expression;
    }

    public Type ElementType => typeof(TElement);

    public Expression Expression { get; }

    public IQueryProvider Provider => provider;

    public override string ToString()
    {
        return provider.GetEvaluatedQuery(Expression);
    }

    public IEnumerator<TElement> GetEnumerator()
    {
        var enumerable = Provider.Execute<TElement>(Expression) as IEnumerable<TElement>
            ?? throw new InvalidOperationException();

        return enumerable.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        var enumerable = Provider.Execute(Expression) as IEnumerable
            ?? throw new InvalidOperationException();

        return enumerable.GetEnumerator();
    }
}
