using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST
{
    public abstract class Declaration : Semantic
    {
        public string Identifier { get; init; }
        public ASTNode InitVal { get; init; }
        /// <summary>
        /// Note: This is the instance of the type of the lexable.
        /// </summary>
        public Type Type { get; init; }
    }
}
