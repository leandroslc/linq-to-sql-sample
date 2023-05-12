namespace LinqToSql.Sample;

public static class Output
{
    public static void Write(IQueryable query, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(query.ToString());
        Console.WriteLine();
        Console.ResetColor();
    }
}
