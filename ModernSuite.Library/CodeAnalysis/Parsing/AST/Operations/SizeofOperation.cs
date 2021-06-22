using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST.Operations
{
    public sealed class SizeofOperation : ASTNode
    {
        public override IEnumerable<ASTNode> Children => Enumerable.Empty<ASTNode>();
        public ModernType ToMeasure { get; init; }
    }

    public sealed class NameofOperation : ASTNode
    {
        public override IEnumerable<ASTNode> Children => Enumerable.Empty<ASTNode>();
        public string Ident { get; init; }
    }
}
