using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Keywords
{
    /// <summary>
    /// The using keyword.
    /// </summary>
    public sealed class UsingKeyword : Keyword
    {
        /// <summary>
        /// Accepts "using".
        /// </summary>
        public override string Representation => "using";
    }
}
