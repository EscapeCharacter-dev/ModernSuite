using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST.Operations
{
    public sealed class ConditionalOperation : ASTNode
    {
        public ASTNode Condition { get; init; }
        public ASTNode IfTrue { get; init; }
        public ASTNode IfFalse { get; init; }
        public override IEnumerable<ASTNode> Children
        {
            get
            {
                yield return Condition;
                yield return IfTrue;
                yield return IfFalse;
            }
        }
    }
}
