using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
