using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST.Statements
{
    public sealed class EmptyStatement : Statement
    {
        public override IEnumerable<Semantic> SubSemantics { get; protected set; }
    }
}
