using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer
{
    public sealed class BadLexable : Lexable
    {
        public override string Representation { get; }
        public BadLexable(string bad)
        {
            Representation = bad;
        }
    }
}
