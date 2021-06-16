using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST.Statements
{
    public sealed class ForStatement : Statement
    {
        public Declaration Declaration { get; init; }
        public ASTNode FirstExpression { get; init; }
        public ASTNode SecondExpression { get; init; }
        public Statement Statement { get; init; }
        public override IEnumerable<Semantic> SubSemantics
        {
            get
            {
                yield return Declaration;
                yield return FirstExpression;
                yield return SecondExpression;
                yield return Statement;
            }
            protected set
            {
                SubSemantics = value;
            }
        }
    }
}
