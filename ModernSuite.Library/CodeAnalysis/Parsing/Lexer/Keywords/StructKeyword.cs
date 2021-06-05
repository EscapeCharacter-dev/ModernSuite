using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Keywords
{
    /// <summary>
    /// Represents the struct keyword.
    /// </summary>
    public sealed class StructKeyword : Keyword
    {
        /// <summary>
        /// Accepts "struct".
        /// </summary>
        public override string Representation => "struct";
    }
}
