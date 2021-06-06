using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST
{
    public abstract class UnaryASTNode : ASTNode
    {
        public ASTNode Child { get; init; }
        public override IEnumerable<ASTNode> Children
        {
            get
            {
                yield return Child;
            }
        }
    }
}
