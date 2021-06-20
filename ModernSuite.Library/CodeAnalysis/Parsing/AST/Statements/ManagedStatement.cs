using ModernSuite.Library.CodeAnalysis.Parsing.AST.Declarations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST.Statements
{
    public sealed class ManagedStatement : Statement
    {
        public ConstantDecl Decl { get; internal set; }
        public Semantic Statement { get; internal set; }

        public override IEnumerable<Semantic> SubSemantics
        {
            get
            {
                yield return Decl;
                yield return Statement;
            }

            protected set
            {
                SubSemantics = value;
            }
        }
    }
}
