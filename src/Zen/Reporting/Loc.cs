namespace Zen.Reporting;

public readonly struct Loc
{
    public readonly int Line;
    public readonly int Column;

    public Loc(int line, int column)
    {
        Line = line;
        Column = column;
    }

    public override string ToString() => $"({Line}, {Column})";
}