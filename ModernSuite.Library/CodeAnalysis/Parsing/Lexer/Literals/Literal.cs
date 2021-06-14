namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Literals
{
    public abstract class Literal : Lexable
    {
        public override string Representation => "";
        public abstract object Value { get; init; }
    }
}
