using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Keywords
{
    public sealed class Identifier : Keyword
    {
        public override string Representation { get; }
        public Identifier(string id) => Representation = id;
    }
}
