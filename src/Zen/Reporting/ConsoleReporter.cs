using System;

namespace Zen.Reporting;

public class ConsoleReporter : IReporter
{
    public void Error(Loc loc, string message) => Console.WriteLine($"{loc}: {message}");
}