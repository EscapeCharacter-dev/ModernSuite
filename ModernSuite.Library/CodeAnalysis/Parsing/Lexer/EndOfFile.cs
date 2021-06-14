namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer
{
    public sealed class EndOfFile : Lexable
    {
        public override string Representation => "\0";
    }
}
