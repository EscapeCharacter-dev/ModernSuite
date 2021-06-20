using System;

namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Keywords
{
    public sealed class KeywordResolver : Resolver<Keyword>
    {
        public override Keyword Parse(string text)
        {
            return text switch
            {
                "bool" => new BoolKeyword(),
                "byte" => new ByteKeyword(),
                "case" => new CaseKeyword(),
                "clang" => new CLangKeyword(),
                "const" => new ConstKeyword(),
                "do" => new DoKeyword(),
                "double" => new DoubleKeyword(),
                "else" => new ElseKeyword(),
                "void" => new VoidKeyword(),
                "foreach" => new ForeachKeyword(),
                "foreachm" => new ForeachmKeyword(),
                "function" => new FunctionKeyword(),
                "goto" => new GotoKeyword(),
                "half" => new HalfKeyword(),
                "if" => new IfKeyword(),
                "int" => new IntKeyword(),
                "least32" => new Least32Keyword(),
                "long" => new LongKeyword(),
                "managed" => new ManagedKeyword(),
                "octa" => new OctaKeyword(),
                "pstruct" => new PStructKeyword(),
                "public" => new PublicKeyword(),
                "quad" => new QuadKeyword(),
                "sbyte" => new SByteKeyword(),
                "short" => new ShortKeyword(),
                "single" => new SingleKeyword(),
                "string" => new StringKeyword(),
                "struct" => new StructKeyword(),
                "switch" => new SwitchKeyword(),
                "uint" => new UIntKeyword(),
                "uleast32" => new ULeast32Keyword(),
                "ulong" => new ULongKeyword(),
                "union" => new UnionKeyword(),
                "uocta" => new UOctaKeyword(),
                "ushort" => new UShortKeyword(),
                "using" => new UsingKeyword(),
                "var" => new VarKeyword(),
                "while" => new WhileKeyword(),
                "for" => new ForKeyword(),
                _ => new Identifier(text)
            };
        }
    }
}
