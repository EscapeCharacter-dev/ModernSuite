namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer
{
    /// <summary>
    /// Represents anything that can be tokenized.
    /// </summary>
    public abstract class Lexable
    {
        /// <summary>
        /// The textual representation of the token.
        /// </summary>
        public abstract string Representation { get; }

        /// <summary>
        /// The line number.
        /// </summary>
        public int Line { get; set; }
        
        /// <summary>
        /// The collumn.
        /// </summary>
        public int Collumn { get; set; }
    }
}
