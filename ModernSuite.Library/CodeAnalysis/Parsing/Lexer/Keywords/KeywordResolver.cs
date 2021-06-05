using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Keywords
{
    public sealed class KeywordResolver : Resolver<Keyword>
    {
        public override Keyword Parse(string text)
        {
            if (text == SyntaxFactory<StructKeyword>.GetRepresentation())
                return new StructKeyword();
            else if (text == SyntaxFactory<UsingKeyword>.GetRepresentation())
                return new UsingKeyword();
            else if (text == SyntaxFactory<VarKeyword>.GetRepresentation())
                return new VarKeyword();
            else
                return new Identifier(text);
        }
    }
}
