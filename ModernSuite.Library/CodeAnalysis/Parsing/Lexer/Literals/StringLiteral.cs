namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Literals
{
    public sealed class StringLiteral : CharactersLiteral
    {
        public override object Value { get; init; }
        public StringLiteral(string str)
        {
            Value = str;
        }
    }
}
