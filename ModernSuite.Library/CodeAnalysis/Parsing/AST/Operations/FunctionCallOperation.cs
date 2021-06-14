using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST.Operations
{
    public sealed class FunctionCallOperation : ASTNode
    {
        public FunctionCallOperation(IEnumerable<ASTNode> parameters)
            => Children = parameters;
        public override IEnumerable<ASTNode> Children { get; }
        public string FuncName { get; init; }
    }
}
