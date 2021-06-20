using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.Scoping
{
    public abstract class Scope
    {
        public string Identifier { get; init; }
        public abstract IEnumerable<Scope> Children { get; }
    }
}
