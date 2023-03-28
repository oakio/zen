namespace Zen.Reporting;

public interface IReporter
{
    void Error(Loc loc, string message);
}