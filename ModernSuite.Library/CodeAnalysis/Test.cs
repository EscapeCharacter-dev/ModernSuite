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
                return (long)Evaluate(a.Left) + (long)Evaluate(a.Right);
            else if (node is SubtractionOperation s)
                return (long)Evaluate(s.Left) - (long)Evaluate(s.Right);
            else if (node is MultiplicationOperation m)
                return (long)Evaluate(m.Left) * (long)Evaluate(m.Right);
            else if (node is DivisionOperation d)
                return (long)Evaluate(d.Left) / (long)Evaluate(d.Right);
            else if (node is RemainderOperation r)
                return (long)Evaluate(r.Left) % (long)Evaluate(r.Right);
            else if (node is NegativeOperation n)
                return -(long)Evaluate(n.Child);
            else if (node is LNotOperation lnot)
                return (long)Evaluate(lnot.Child) != 0 ? 0 : 1;
            else if (node is BNotOperation b)
                return ~(long)Evaluate(b.Child);
            else if (node is ParenthesizedOperation p)
                return Convert.ToInt64(Evaluate(p.Child));
            else if (node is BinaryLeftShiftOperation blso)
                return (long)Evaluate(blso.Left) << Convert.ToInt32((long)Evaluate(blso.Right));
            else if (node is BinaryRightShiftOperation brso)
                return (long)Evaluate(brso.Left) >> Convert.ToInt32((long)Evaluate(brso.Left));
            else if (node is LowerOperation lo)
                return (long)Evaluate(lo.Left) < (long)Evaluate(lo.Right) ? 1 : 0;
            else if (node is GreaterOperation go)
                return (long)Evaluate(go.Left) > (long)Evaluate(go.Right) ? 1 : 0;
            else if (node is LowerEqualOperation leo)
                return (long)Evaluate(leo.Left) <= (long)Evaluate(leo.Right) ? 1 : 0;
            else if (node is GreaterEqualOperation geo)
                return (long)Evaluate(geo.Left) >= (long)Evaluate(geo.Right) ? 1 : 0;
            else if (node is EqualityOperation eo)
                return (long)Evaluate(eo.Left) == (long)Evaluate(eo.Right) ? 1 : 0;
            else if (node is NotEqualOperation neo)
                return (long)Evaluate(neo.Left) != (long)Evaluate(neo.Right) ? 1 : 0;
            else
                return null;
        }
    }
}
