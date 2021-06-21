using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST.Declarations
{
    public sealed class EnumDecl : Declaration
    {
        public List<string> EnumDecls { get; init; } = new List<string>();
    }
}
