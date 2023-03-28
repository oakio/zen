using System.IO;
using Antlr4.Runtime;
using Zen.AST;
using Zen.Reporting;

namespace Zen.Antlr;

public class ZenAstBuilder : IAstBuilder
{
    private readonly IReporter _reporter;

    public ZenAstBuilder(IReporter reporter)
    {
        _reporter = reporter;
    }

    public bool TryBuild(string sourceName, StringReader reader, out IAstNode ast)
    {
        var input = new AntlrInputStream(reader) { name = sourceName };

        var lexer = new ZenLexer(input);
        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(new ZenLangLexerErrorListener(_reporter));

        var tokenStream = new CommonTokenStream(lexer);
        var parser = new ZenParser(tokenStream);
        parser.RemoveErrorListeners();
        parser.AddErrorListener(new ZenParserErrorListener(_reporter));

        ZenParser.ModuleContext context = parser.module();
        var builder = new ZenVisitor();

        ast = builder.Visit(context);
        return parser.NumberOfSyntaxErrors == 0;
    }
}