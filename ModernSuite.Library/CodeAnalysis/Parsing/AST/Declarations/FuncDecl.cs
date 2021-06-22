using System.Collections.Generic;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST.Declarations
{
    public sealed class FuncDecl : Declaration
    {
        public IEnumerable<Declaration> Parameters { get; init; }
        public Semantic Code { get; init; }
    }
}
