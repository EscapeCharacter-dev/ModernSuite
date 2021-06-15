using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST.Statements
{
    public sealed class WhileStatement : Statement
    {
        public ASTNode Expression { get; init; }
        public Semantic While { get; init; }
        public override IEnumerable<Semantic> SubSemantics
        {
            get
            {
                yield return Expression;
                yield return While;
            }
            protected set
            {
                SubSemantics = value;
            }
        }
    }
}
