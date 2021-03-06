using System.Collections.Generic;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST
{
    public abstract class ASTNode : Semantic
    {
        public abstract IEnumerable<ASTNode> Children { get; }
    }
}
