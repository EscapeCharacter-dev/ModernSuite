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
            else
                return null;
        }
    }
}
