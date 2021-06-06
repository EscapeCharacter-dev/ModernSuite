using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST
{
    public abstract class ASTNode
    {
        public abstract IEnumerable<ASTNode> Children { get; }
    }
}
