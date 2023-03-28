using Antlr4.Runtime;
using Zen.Reporting;

namespace Zen.Antlr;

public class ZenLangLexerErrorListener : IAntlrErrorListener<int>
{
    private readonly IReporter _reporter;

    public ZenLangLexerErrorListener(IReporter reporter)
    {
        _reporter = reporter;
    }

    public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg,
        RecognitionException e)
    {
        var loc = new Loc(line, charPositionInLine);
        _reporter.Error(loc, msg);
    }
}