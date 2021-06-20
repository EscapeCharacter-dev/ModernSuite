using ModernSuite.Library.CodeAnalysis.Parsing.Scoping;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST
{
    /// <summary>
    /// Represents a Modern source file.
    /// </summary>
    public sealed class ModernProgram : Declaration
    {

        /// <summary>
        /// The semantics in the source code.
        /// </summary>
        private List<Semantic> _semantics = new List<Semantic>();

        /// <summary>
        /// Gets an immutable array of semantics.
        /// </summary>
        /// <returns>The semantics in the source code.</returns>
        public ImmutableArray<Semantic> GetSemantics()
            => _semantics.ToImmutableArray();

        internal void SetSemantics(List<Semantic> semantics)
            => _semantics = semantics;

        /// <summary>
        /// Creates a new instance of ModernProgram.
        /// </summary>
        /// <param name="semantics">The semantics that have been parsed.</param>
        public ModernProgram(List<Semantic> semantics)
        {
            _semantics = semantics;
        }
    }
}
