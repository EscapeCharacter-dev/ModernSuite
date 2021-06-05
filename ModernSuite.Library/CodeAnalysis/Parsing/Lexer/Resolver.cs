using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer
{
    public abstract class Resolver<T> where T : Lexable
    {
        public abstract T Parse(string text);
    }
}
