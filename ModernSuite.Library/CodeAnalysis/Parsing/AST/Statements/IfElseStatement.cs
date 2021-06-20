using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST.Statements
{
    public sealed class IfElseStatement : Statement
    {
        public ASTNode Expression { get; init; }
        public Semantic TrueCode { get; internal set; }
        public Semantic ElseCode { get; internal set; }
        public override IEnumerable<Semantic> SubSemantics
        {
            get
            {
                yield return Expression;
                yield return TrueCode;
                yield return ElseCode;
            }

            protected set
            {
                SubSemantics = value;
            }
        }
    }
}
