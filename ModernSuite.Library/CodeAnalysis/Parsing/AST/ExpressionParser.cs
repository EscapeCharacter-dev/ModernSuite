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
        public Lexable Current => Position < Lexables.Count ? Lexables[Position] : null;

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

            var token = Current;

            if (token is Literal asLiteral)
            {
                Position++;
                return new LiteralASTNode { Lexable = asLiteral };
            }
            else if (token is MinusOperator)
            {
                Position++;
                return new NegativeOperation { Child = ParsePrimary() };
            }
            else if (token is BangOperator)
            {
                Position++;
                return new LNotOperation { Child = ParsePrimary() };
            }
            else if (token is TildaOperator)
            {
                Position++;
                return new BNotOperation { Child = ParsePrimary() };
            }
            else if (token is ParenthesisOpenOperator)
            {
                Position++;
                var tree = Parse();
                if (Position >= Lexables.Count)
                {
                    Console.WriteLine("Missing closing parenthesis (premature EOL).");
                    return null;
                }
                if (Current is ParenthesisClosedOperator)
                {
                    Position++;
                    return new ParenthesizedOperation { Child = tree };
                }
                else
                {
                    Console.WriteLine("Missing closing parenthesis.");
                    return null;
                }
            }
            else if (token is PlusOperator)
            {
                Position++;
                return new UnaryPlusOperation { Child = ParsePrimary() };
            }
            else if (token is AtOperator)
            {
                Position++;
                return new AddressOfOperation { Child = ParsePrimary() };
            }
            else if (token is DollarOperator)
            {
                Position++;
                return new ValueOfOperation { Child = ParsePrimary() };
            }
            else
            {
                Console.WriteLine($"{token.GetType()} is not a valid literal value!");
                return null;
            }
        }

        private ASTNode ParseFactor()
        {
            var left = ParsePrimary();
            if (Position >= Lexables.Count)
            {
                return left;
            }

            while (Current is FactorialOperator)
            {
                var op = Current;
                if (op is not Operator)
                {
                    Console.WriteLine($"Expected an operator, instead got {op.GetType()}");
                    return null;
                }
                Position++;
                var right = ParseFactor();
                left = op is StarOperator ? new MultiplicationOperation { Left = left, Right = right } :
                    op is SlashOperator ? new DivisionOperation { Left = left, Right = right } :
                    op is PercentageOperator ? new RemainderOperation { Left = left, Right = right } :
                    null;
            }
            return left;
        }

        private ASTNode ParseAdditive()
        {
            var left = ParseFactor();
            if (Position >= Lexables.Count)
            {
                return left;
            }
            while (Current is AdditiveOperator)
            {
                if (Position >= Lexables.Count)
                {
                    return left;
                }
                var op = Current;
                if (op is not Operator)
                {
                    Console.WriteLine($"Expected an operator, instead got {op.GetType()}");
                    return null;
                }
                Position++;
                var right = ParseAdditive();
                left = op is PlusOperator ? new AdditionOperation { Left = left, Right = right } :
                    op is MinusOperator ? new SubtractionOperation { Left = left, Right = right } :
                    null;
            }
            return left;
        }

        private ASTNode ParseShifts()
        {
            var left = ParseAdditive();
            if (Position >= Lexables.Count)
            {
                return left;
            }
            while (Current is ArrowsLeftOperator || Current is ArrowsRightOperator)
            {
                if (Position >= Lexables.Count)
                {
                    return left;
                }
                var op = Current;
                if (op is not Operator)
                {
                    Console.WriteLine($"Expected an operator, instead got {op.GetType()}");
                    return null;
                }
                Position++;
                var right = ParseShifts();
                left = op is ArrowsLeftOperator ? new BinaryLeftShiftOperation { Left = left, Right = right } :
                    op is ArrowsRightOperator ? new BinaryRightShiftOperation { Left = left, Right = right } :
                    null;
            }
            return left;
        }

        private ASTNode ParseRelationals()
        {
            var left = ParseShifts();
            if (Position >= Lexables.Count)
            {
                return left;
            }
            while (Current is ArrowLeftOperator || Current is ArrowRightOperator ||
                   Current is ArrowEqualLeftOperator || Current is ArrowEqualRightOperator)
            {
                if (Position >= Lexables.Count)
                {
                    return left;
                }
                var op = Current;
                if (op is not Operator)
                {
                    Console.WriteLine($"Expected an operator, instead got {op.GetType()}");
                    return null;
                }
                Position++;
                var right = ParseRelationals();
                left = op is ArrowLeftOperator ? new LowerOperation { Left = left, Right = right } :
                    op is ArrowRightOperator ? new GreaterOperation { Left = left, Right = right } :
                    op is ArrowEqualLeftOperator ? new LowerEqualOperation { Left = left, Right = right } :
                    op is ArrowEqualRightOperator ? new GreaterEqualOperation { Left = left, Right = right } :
                    null;
            }
            return left;
        }

        private ASTNode ParseEqualities()
        {
            var left = ParseRelationals();
            if (Position >= Lexables.Count)
            {
                return left;
            }
            while (Current is EqualsOperator || Current is BangEqualOperator)
            {
                if (Position >= Lexables.Count)
                {
                    return left;
                }
                var op = Current;
                if (op is not Operator)
                {
                    Console.WriteLine($"Expected an operator, instead got {op.GetType()}");
                    return null;
                }
                Position++;
                var right = ParseEqualities();
                left = op is EqualsOperator ? new EqualityOperation { Left = left, Right = right } :
                    op is BangEqualOperator ? new NotEqualOperation { Left = left, Right = right } :
                    null;
            }
            return left;
        }

        private ASTNode ParseBAnds()
        {
            var left = ParseEqualities();
            if (Position >= Lexables.Count)
            {
                return left;
            }
            while (Current is AndOperator)
            {
                if (Position >= Lexables.Count)
                {
                    return left;
                }
                var op = Current;
                if (op is not Operator)
                {
                    Console.WriteLine($"Expected an operator, instead got {op.GetType()}");
                    return null;
                }
                Position++;
                var right = ParseBAnds();
                left = new BAndOperation { Left = left, Right = right };
            }
            return left;
        }

        private ASTNode ParseBXors()
        {
            var left = ParseBAnds();
            if (Position >= Lexables.Count)
            {
                return left;
            }
            while (Current is HelmetOperator)
            {
                if (Position >= Lexables.Count)
                {
                    return left;
                }
                var op = Current;
                if (op is not Operator)
                {
                    Console.WriteLine($"Expected an operator, instead got {op.GetType()}");
                    return null;
                }
                Position++;
                var right = ParseBXors();
                left = new BXorOperation { Left = left, Right = right };
            }
            return left;
        }

        private ASTNode ParseBOrs()
        {
            var left = ParseBXors();
            if (Position >= Lexables.Count)
            {
                return left;
            }
            while (Current is PipeOperator)
            {
                if (Position >= Lexables.Count)
                {
                    return left;
                }
                var op = Current;
                if (op is not Operator)
                {
                    Console.WriteLine($"Expected an operator, instead got {op.GetType()}");
                    return null;
                }
                Position++;
                var right = ParseBOrs();
                left = new BOrOperation { Left = left, Right = right };
            }
            return left;
        }

        private ASTNode ParseLAnds()
        {
            var left = ParseBOrs();
            if (Position >= Lexables.Count)
            {
                return left;
            }
            while (Current is AndsOperator)
            {
                if (Position >= Lexables.Count)
                {
                    return left;
                }
                var op = Current;
                if (op is not Operator)
                {
                    Console.WriteLine($"Expected an operator, instead got {op.GetType()}");
                    return null;
                }
                Position++;
                var right = ParseLAnds();
                left = new LAndOperation { Left = left, Right = right };
            }
            return left;
        }

        private ASTNode ParseLOrs()
        {
            var left = ParseLAnds();
            if (Position >= Lexables.Count)
            {
                return left;
            }
            while (Current is PipesOperator)
            {
                if (Position >= Lexables.Count)
                {
                    return left;
                }
                var op = Current;
                if (op is not Operator)
                {
                    Console.WriteLine($"Expected an operator, instead got {op.GetType()}");
                    return null;
                }
                Position++;
                var right = ParseLOrs();
                left = new LOrOperation { Left = left, Right = right };
            }
            return left;
        }

        public ASTNode Parse()
        {
            return ParseLOrs();
        }
    }
}
