using LinqToSql.Core;
using LinqToSql.Sample;
using static LinqToSql.Sample.Output;

var provider = new DbQueryProvider();
var query = new DbCollection<Person>(provider);

// 1st example
Write(
    query,
    ConsoleColor.Yellow);

Write(
    from p in query select p,
    ConsoleColor.Green);

// 2nd example
Write(
    query.Where(p => p.Age > 25),
    ConsoleColor.Yellow);

Write(
    from p in query where p.Age > 25 select p,
    ConsoleColor.Green);

// 3rd example
Write(
    query.Where(p => p.Name!.Contains("Leandro")),
    ConsoleColor.Yellow);

Write(
    from p in query where p.Name!.Contains("Leandro") select p,
    ConsoleColor.Green);

// 4th example
Write(
    query.Where(p => p.Name!.Contains("Leandro") || p.Age > 25),
    ConsoleColor.Yellow);

Write(
    from p in query where p.Name!.Contains("Leandro") || p.Age > 25 select p,
    ConsoleColor.Green);
