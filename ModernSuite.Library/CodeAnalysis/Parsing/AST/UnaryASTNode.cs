using System.Collections.Generic;

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
