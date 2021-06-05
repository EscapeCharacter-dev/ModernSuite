﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Literals
{
    public sealed class BooleanLiteral : Literal
    {
        public override object Value { get; init; }
    }
}