using Antlr4.Runtime;
using Zen.Reporting;

namespace Zen.Antlr;

public class ZenParserErrorListener : BaseErrorListener
{
    private readonly IReporter _reporter;

    public ZenParserErrorListener(IReporter reporter)
    {
        _reporter = reporter;
    }

    public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg,
        RecognitionException e)
    {
        var loc = new Loc(line, charPositionInLine);
        _reporter.Error(loc, msg);
    }
}