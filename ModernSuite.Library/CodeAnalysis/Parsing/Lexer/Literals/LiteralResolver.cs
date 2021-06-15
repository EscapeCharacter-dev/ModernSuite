namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Literals
{
    public sealed class LiteralResolver : Resolver<Literal>
    {
        public override Literal Parse(string text)
        {
            if (long.TryParse(text, out var i64Result))
                return new IntLiteral { Value = i64Result };
            else if (text == "true")
                return new BooleanLiteral { Value = true };
            else if (text == "false")
                return new BooleanLiteral { Value = false };
            else
                return null;
        }
    }
}
