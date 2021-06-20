using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST.Statements
{
    public sealed class DoWhileStatement : Statement
    {
        public ASTNode Expression { get; internal set; }
        public Semantic Code { get; internal set; }

        public override IEnumerable<Semantic> SubSemantics
        {
            get
            {
                yield return Expression;
                yield return Code;
            }
            protected set
            {
                SubSemantics = value;
            }
        }
    }
}
