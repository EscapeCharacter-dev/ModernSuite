namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer
{
    public static class SyntaxFactory<T>
        where T : Lexable, new()
    {
        public static string GetRepresentation()
            => new T().Representation;
    }
}
