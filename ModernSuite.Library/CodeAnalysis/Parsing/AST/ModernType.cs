using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST
{
    public enum ModernTypeKind
    {
        SByte,
        Byte,
        Short,
        UShort,
        Int,
        UInt,
        Least32,
        ULeast32,
        Long,
        ULong,
        Single,
        Double,
        Quad,
        Pointer,
        Void,
        Function,
        Array,
    }
    public sealed class ModernType : Semantic
    {
        public ModernTypeKind Kind { get; init; }
        public ModernType ChildType { get; init; }
        public ASTNode Optional { get; init; }
    }
}
