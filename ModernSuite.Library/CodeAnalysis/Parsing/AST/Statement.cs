using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST
{
    /// <summary>
    /// This class represents a statement like if or goto.
    /// </summary>
    public abstract class Statement : Semantic
    {
        public abstract IEnumerable<Semantic> SubSemantics { get; }
    }
}
