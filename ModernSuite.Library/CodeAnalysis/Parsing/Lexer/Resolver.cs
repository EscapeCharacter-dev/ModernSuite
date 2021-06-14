namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer
{
    public abstract class Resolver<T> where T : Lexable
    {
        public abstract T Parse(string text);
    }
}
