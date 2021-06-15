namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Literals
{
    public abstract class Literal : Lexable
    {
        public override string Representation { get => Value.ToString(); }
        public abstract object Value { get; init; }
    }
}
