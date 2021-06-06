using ModernSuite.Library.CodeAnalysis.Parsing.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST
{
    public sealed class LiteralASTNode : ASTNode
    {
        public Lexable Lexable { get; init; }
        public override IEnumerable<ASTNode> Children => Enumerable.Empty<ASTNode>();
    }
}
