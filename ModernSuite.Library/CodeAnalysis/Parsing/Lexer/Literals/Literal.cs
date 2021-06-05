using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Literals
{
    public abstract class Literal : Lexable
    {
        public override string Representation => "";
        public abstract object Value { get; init; }
    }
}
