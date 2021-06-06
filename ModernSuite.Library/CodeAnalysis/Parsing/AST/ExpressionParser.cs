using ModernSuite.Library.CodeAnalysis.Parsing.AST.Operations;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Literals;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST
{
    public sealed class ExpressionParser
    {
        /// <summary>
        /// The list of tokens.
        /// </summary>
        public List<Lexable> Lexables { get; } = new List<Lexable>();
        public int Position { get; private set; } = 0;

        public ExpressionParser(string text)
        {
            var tokenizer = new Tokenizer(text);

            while (true)
            {
                var token = tokenizer.NextToken();

                if (token is EndOfFile || token is BadLexable)
                    break;

                Lexables.Add(token);
            }
        }

        private ASTNode ParsePrimary()
        {
            if (Position >= Lexables.Count)
            {
                Console.WriteLine("There is no more tokens left in the buffer.");
                return null;
            }

            var token = Lexables[Position];

            if (token is Literal asLiteral)
            {
                Position++;
                return new LiteralASTNode { Lexable = asLiteral };
            }
            else
            {
                Console.WriteLine($"{token.GetType()} is not a valid literal value!");
                return null;
            }
        }

        private ASTNode ParseAdditive()
        {
            var left = ParseFactor();
            if (Position >= Lexables.Count)
            {
                return left;
            }
            var op = Lexables[Position];
            if (op is not Operator)
            {
                Console.WriteLine($"Expected an operator, instead got {op.GetType()}");
                return null;
            }
            Position++;
            var right = ParseFactor();
            return op is PlusOperator ?
                   new AdditionOperation { Left = left, Right = right } : op is MinusOperator ?
                   new SubtractionOperation { Left = left, Right = right } : null;
        }

        private ASTNode ParseFactor()
        {
            var left = ParsePrimary();
            if (Position >= Lexables.Count)
            {
                return left;
            }
            var op = Lexables[Position];
            if (op is not Operator)
            {
                Console.WriteLine($"Expected an operator, instead got {op.GetType()}");
                return null;
            }
            Position++;
            var right = ParsePrimary();
            return op is StarOperator ?
                   new MultiplicationOperation { Left = left, Right = right } : op is SlashOperator ?
                   new DivisionOperation { Left = left, Right = right } : op is PercentageOperator ?
                   new RemainderOperation { Left = left, Right = right } : null;
        }

        public ASTNode Parse()
        {
            return ParseAdditive();
        }
    }
}
