using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST.Statements
{
    public sealed class GotoStatement : Statement
    {
        public ASTNode Objective { get; init; }
        public override IEnumerable<Semantic> SubSemantics
        {
            get
            {
                yield return Objective;
            }
        }
    }
}
