﻿using System.Collections.Generic;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST.Statements
{
    public sealed class BreakStatement : Statement
    {
        public override IEnumerable<Semantic> SubSemantics { get; protected set; }
    }
}
