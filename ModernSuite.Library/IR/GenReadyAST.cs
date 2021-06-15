using ModernSuite.Library.CodeAnalysis.Parsing.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.IR
{
    public sealed class GenReadyAST
    {
        public ASTNode Node { get; init; }
        public int OpCounter { get; init; }

        public static implicit operator ASTNode(GenReadyAST grast)
            => grast.Node;

        public static implicit operator int(GenReadyAST grast)
            => grast.OpCounter;
    }
}
