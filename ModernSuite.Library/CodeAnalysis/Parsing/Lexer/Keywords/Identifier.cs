namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Keywords
{
    public sealed class Identifier : Keyword
    {
        public Identifier(string ident)
            => Representation = ident;
        public override string Representation { get; } = "null_ident";
    }
}
