using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer
{
    public sealed class EndOfFile : Lexable
    {
        public override string Representation => "\0";
    }
}
