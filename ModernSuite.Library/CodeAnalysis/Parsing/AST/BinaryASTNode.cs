using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST
{
    public abstract class BinaryASTNode : ASTNode
    {
        public ASTNode Left { get; init; }
        public ASTNode Right { get; init; }
        public override IEnumerable<ASTNode> Children
        {
            get
            {
                yield return Left;
                yield return Right;
            }
        }
    }
}
