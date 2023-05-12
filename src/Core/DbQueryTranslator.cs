using System.Linq.Expressions;
using System.Text;

namespace LinqToSql.Core;

public class DbQueryTranslator : ExpressionVisitor
{
    private QueryBuilder builder = new();
    private StringBuilder currentBuilder = new();

    public (string, Dictionary<string, object?>) Translate(Expression expression)
    {
        Visit(expression);

        return (builder.Build(), builder.Parameters);
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        return node.Method.DeclaringType == typeof(Queryable)
            ? VisitQueryableMethodCall(node)
            : VisitNormalMethodCall(node);
    }

    protected internal Expression VisitQueryableMethodCall(MethodCallExpression node)
    {
        currentBuilder = new();

        if (node.Method.Name == nameof(Queryable.Where))
        {
            var whereNode = (LambdaExpression)GetOperand(node.Arguments[1]);

            Visit(whereNode.Body);

            builder.WhereExpressions.Add(currentBuilder.ToString());

            return Visit(node.Arguments[0]);
        }

        if (node.Method.Name == nameof(Queryable.Select))
        {
            return Visit(node.Arguments[0]);
        }

        throw new NotSupportedException($"Method {node.Method.Name} not supported");
    }

    protected internal Expression VisitNormalMethodCall(MethodCallExpression node)
    {
        if (IsStatic(node) is false)
        {
            if (node.Object is MemberExpression member
                && IsStatic(member) is false
                && member.Expression!.NodeType == ExpressionType.Parameter)
            {
                if (node.Method.DeclaringType == typeof(string)
                                    && node.Method.Name == "Contains")
                {
                    Visit(member);

                    currentBuilder.Append(" LIKE '%' + ");

                    Visit(node.Arguments[0]);

                    currentBuilder.Append(" + '%'");

                    return node;
                }
            }
        }

        throw new NotSupportedException($"Method {node.Method.Name} not supported");
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        currentBuilder.Append("(");

        Visit(node.Left);

        currentBuilder.Append($" {TranslateBinary(node)} ");

        Visit(node.Right);

        currentBuilder.Append(")");

        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (IsStatic(node) is false
            && node.Expression!.NodeType == ExpressionType.Parameter)
        {
            currentBuilder.Append(node.Member.Name);

            return node;
        }

        throw new NotSupportedException($"Member {node.Member.Name} not supported");
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        if (node.Value is IQueryable queryable)
        {
            builder.TableName = queryable.ElementType.Name;

            return node;
        }

        var paramName = builder.AddParameter(node.Value);

        currentBuilder.Append(paramName);

        return node;
    }

    private static string TranslateBinary(BinaryExpression node)
    {
        return node.NodeType switch
        {
            ExpressionType.AndAlso => "AND",
            ExpressionType.OrElse => "OR",
            ExpressionType.GreaterThan => ">",
            _ => throw new NotSupportedException(
                $"Binary operator {node.NodeType} not supported"),
        };
    }

    private static Expression GetOperand(Expression node)
    {
        while (node.NodeType == ExpressionType.Quote)
        {
            node = ((UnaryExpression)node).Operand;
        }

        return node;
    }

    private static bool IsStatic(Expression node)
    {
        return node is MemberExpression member
            ? member.Expression == null
            : node is MethodCallExpression method
                ? method.Object == null
                : false;
    }
}
