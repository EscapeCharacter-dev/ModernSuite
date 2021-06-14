using ModernSuite.Library.CodeAnalysis.Parsing.AST;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Operations;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Literals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis
{
    public class Test
    {
        public void ParseTest()
        {
            while (true)
            {
                var parser = new ExpressionParser(Console.ReadLine());
                Console.WriteLine($"{Evaluate(parser.Parse())}");
            }

        }

        private object Evaluate(ASTNode node)
        {
            if (node is LiteralASTNode l)
                return (l.Lexable as Literal).Value;
            else if (node is AdditionOperation a)
                return Convert.ToInt64(Evaluate(a.Left)) + Convert.ToInt64(Evaluate(a.Right));
            else if (node is SubtractionOperation s)
                return Convert.ToInt64(Evaluate(s.Left)) - Convert.ToInt64(Evaluate(s.Right));
            else if (node is MultiplicationOperation m)
                return Convert.ToInt64(Evaluate(m.Left)) * Convert.ToInt64(Evaluate(m.Right));
            else if (node is DivisionOperation d)
                return Convert.ToInt64(Evaluate(d.Left)) / Convert.ToInt64(Evaluate(d.Right));
            else if (node is RemainderOperation r)
                return Convert.ToInt64(Evaluate(r.Left)) % Convert.ToInt64(Evaluate(r.Right));
            else if (node is NegativeOperation n)
                return -Convert.ToInt64(Evaluate(n.Child));
            else if (node is LNotOperation lnot)
                return Convert.ToInt64(Evaluate(lnot.Child)) != 0 ? 0 : 1;
            else if (node is BNotOperation b)
                return ~Convert.ToInt64(Evaluate(b.Child));
            else if (node is ParenthesizedOperation p)
                return Convert.ToInt64(Evaluate(p.Child));
            else if (node is BinaryLeftShiftOperation blso)
                return Convert.ToInt64(Evaluate(blso.Left)) << Convert.ToInt32(Evaluate(blso.Right));
            else if (node is BinaryRightShiftOperation brso)
                return Convert.ToInt64(Evaluate(brso.Left)) >> Convert.ToInt32(Evaluate(brso.Left));
            else if (node is LowerOperation lo)
                return Convert.ToInt64(Evaluate(lo.Left)) < Convert.ToInt64(Evaluate(lo.Right)) ? 1 : 0;
            else if (node is GreaterOperation go)
                return Convert.ToInt64(Evaluate(go.Left)) > Convert.ToInt64(Evaluate(go.Right)) ? 1 : 0;
            else if (node is LowerEqualOperation leo)
                return Convert.ToInt64(Evaluate(leo.Left)) <= Convert.ToInt64(Evaluate(leo.Right)) ? 1 : 0;
            else if (node is GreaterEqualOperation geo)
                return Convert.ToInt64(Evaluate(geo.Left)) >= Convert.ToInt64(Evaluate(geo.Right)) ? 1 : 0;
            else if (node is EqualityOperation eo)
                return Convert.ToInt64(Evaluate(eo.Left)) == Convert.ToInt64(Evaluate(eo.Right)) ? 1 : 0;
            else if (node is NotEqualOperation neo)
                return Convert.ToInt64(Evaluate(neo.Left)) != Convert.ToInt64(Evaluate(neo.Right)) ? 1 : 0;
            else if (node is BAndOperation bao)
                return Convert.ToInt64(Evaluate(bao.Left)) & Convert.ToInt64(Evaluate(bao.Right));
            else if (node is BXorOperation bxo)
                return Convert.ToInt64(Evaluate(bxo.Left)) ^ Convert.ToInt64(Evaluate(bxo.Right));
            else if (node is BOrOperation boo)
                return Convert.ToInt64(Evaluate(boo.Left)) | Convert.ToInt64(Evaluate(boo.Right));
            else if (node is LAndOperation lao)
                return Convert.ToInt64(Evaluate(lao.Left)) != 0 && Convert.ToInt64(Evaluate(lao.Right)) != 0 ? 1 : 0;
            else if (node is LOrOperation loo)
                return Convert.ToInt64(Evaluate(loo.Left)) != 0 || Convert.ToInt64(Evaluate(loo.Right)) != 0 ? 1 : 0;
            else if (node is UnaryPlusOperation upo)
                return Convert.ToInt64(Evaluate(upo.Child));
            else
                return null;
        }
    }
}
