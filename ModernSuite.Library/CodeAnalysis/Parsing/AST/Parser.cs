using ModernSuite.Library.CodeAnalysis.Parsing.AST.Declarations;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Operations;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Statements;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Keywords;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Literals;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Operators;
using System;
using System.Collections.Generic;

namespace ModernSuite.Library.CodeAnalysis.Parsing.AST
{
    public sealed class Parser
    {
        /// <summary>
        /// The list of tokens.
        /// </summary>
        public List<Lexable> Lexables { get; } = new List<Lexable>();
        public int Position { get; private set; } = 0;
        public Lexable Current => Position < Lexables.Count ? Lexables[Position] : null;
        public Lexable PeekNext => Position + 1 < Lexables.Count ? Lexables[Position + 1] : null;

        public Parser(string text)
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
                var tree = ParseLOrs();
                if (Position >= Lexables.Count)
                {
                    Console.WriteLine($"({token.Line},{token.Collumn}) Missing closing parenthesis (premature EOL).");
                    return null;
                }
                if (Current is ParenthesisClosedOperator)
                {
                    Position++;
                    return new ParenthesizedOperation { Child = tree };
                }
                else
                {
                    Console.WriteLine($"({token.Line},{token.Collumn}) Missing closing parenthesis.");
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
            else if (token is SizeofKeyword)
            {
                Position++;
                return new SizeofOperation { ToMeasure = ParseType() as ModernType };
            }
            else if (token is DollarOperator)
            {
                Position++;
                return new ValueOfOperation { Child = ParsePrimary() };
            }
            else if (token is Identifier i)
            {
                if (PeekNext is ParenthesisOpenOperator)
                {
                    Position += 2;
                    var parameters = new List<ASTNode>();
                    while (Current is not ParenthesisClosedOperator)
                    {
                        parameters.Add(ParseLOrs());
                        if (Current is not CommaOperator)
                            break;
                        Position++;
                    }
                    Position++;
                    return new FunctionCallOperation(parameters) { FuncName = i.Representation };
                }
                Position++;
                return new IdentifierOperation { IdentName = i.Representation };
            }
            else
            {
                Console.WriteLine($"Token at ({token.Line},{token.Collumn}) is not a valid literal value!");
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
                    Console.WriteLine($"Expected an operator at ({Current.Line},{Current.Collumn})");
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
                return left;
            while (Current is AdditiveOperator)
            {
                if (Position >= Lexables.Count)
                    return left;
                var op = Current;
                if (op is not Operator)
                {
                    Console.WriteLine($"Expected an operator at ({Current.Line},{Current.Collumn})");
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
                return left;
            while (Current is ArrowsLeftOperator || Current is ArrowsRightOperator)
            {
                if (Position >= Lexables.Count)
                    return left;
                var op = Current;
                if (op is not Operator)
                {
                    Console.WriteLine($"Expected an operator at ({Current.Line},{Current.Collumn})");
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
                return left;
            while (Current is ArrowLeftOperator || Current is ArrowRightOperator ||
                   Current is ArrowEqualLeftOperator || Current is ArrowEqualRightOperator)
            {
                if (Position >= Lexables.Count)
                    return left;
                var op = Current;
                if (op is not Operator)
                {
                    Console.WriteLine($"Expected an operator at ({Current.Line},{Current.Collumn})");
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
                return left;
            while (Current is EqualsOperator || Current is BangEqualOperator)
            {
                if (Position >= Lexables.Count)
                    return left;
                var op = Current;
                if (op is not Operator)
                {
                    Console.WriteLine($"Expected an operator at ({Current.Line},{Current.Collumn})");
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
                return left;
            while (Current is AndOperator)
            {
                if (Position >= Lexables.Count)
                    return left;
                var op = Current;
                if (op is not Operator)
                {
                    Console.WriteLine($"Expected an operator at ({Current.Line},{Current.Collumn})");
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
                return left;
            while (Current is HelmetOperator)
            {
                if (Position >= Lexables.Count)
                    return left;
                var op = Current;
                if (op is not Operator)
                {
                    Console.WriteLine($"Expected an operator at ({Current.Line},{Current.Collumn})");
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
                    Console.WriteLine($"Expected an operator at ({Current.Line},{Current.Collumn})");
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
                    Console.WriteLine($"Expected an operator at ({Current.Line},{Current.Collumn})");
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
                    Console.WriteLine($"Expected an operator at ({Current.Line},{Current.Collumn})");
                    return null;
                }
                Position++;
                var right = ParseLOrs();
                left = new LOrOperation { Left = left, Right = right };
            }
            return left;
        }

        private Semantic ParseStatement()
        {
            if (Current is IfKeyword)
            {
                Position++;
                if (Current is ParenthesisOpenOperator)
                {
                    Position++;
                    var expr = ParseLOrs();
                    if (Current is not ParenthesisClosedOperator)
                    {
                        Console.WriteLine($"({Current.Line},{Current.Collumn}) Missing closing parenthesis in if statement");
                        return null;
                    }
                    Position++;
                    var code = Parse();
                    Semantic else_code = null;
                    if (Current is ElseKeyword)
                    {
                        Position++;
                        else_code = Parse();
                    }
                    return new IfElseStatement { TrueCode = code, ElseCode = else_code, Expression = expr };
                }
                Console.WriteLine($"({Current.Line},{Current.Collumn}) Invalid if statement syntax");
                return null;
            }
            else if (Current is GotoKeyword)
            {
                Position++;
                var objective = ParseLOrs();
                if (objective is null)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Invalid goto syntax");
                    return null;
                }
                if (Current is not SemicolonOperator)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Expected a semicolon");
                    return null;
                }
                Position++;
                return new GotoStatement { Objective = objective };
            }
            else if (Current is ManagedKeyword)
            {
                Position++;
                if (Current is not ParenthesisOpenOperator)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Incorrect usage of managed statement");
                    return null;
                }
                Position++;
                var decl = ParseDeclaration();
                if (decl is not ConstantDecl)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) A managed pointer must be constant");
                    return null;
                }
                Position++;
                if (Current is not ParenthesisClosedOperator)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Expected closed parenthesis at the end of managed statement");
                }
                Position++;
                var code = ParseStatement();
                return new ManagedStatement { Decl = decl as ConstantDecl, Statement = code };
            }
            else if (Current is WhileKeyword)
            {
                Position++;
                if (Current is ParenthesisOpenOperator)
                {
                    Position++;
                    var expr = ParseLOrs();
                    if (Current is not ParenthesisClosedOperator)
                    {
                        Console.WriteLine($"({Current.Line},{Current.Collumn}) Missing closing parenthesis in if statement");
                        return null;
                    }
                    Position++;
                    var code = Parse();
                    return new WhileStatement { Expression = expr, While = code };
                }
                Console.WriteLine($"({Current.Line},{Current.Collumn}) Invalid while statement syntax");
                return null;
            }
            else if (Current is DoKeyword)
            {
                Position++;
                var code = Parse();
                if (Current is not WhileKeyword)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Invalid do...while statement syntax");
                    return null;
                }
                Position++;
                if (Current is not ParenthesisOpenOperator)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Invalid do...while statement syntax");
                    return null;
                }
                Position++;
                var expr = ParseLOrs();
                if (Current is not ParenthesisClosedOperator)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Missing closing parenthesis in do...while statement");
                    return null;
                }
                Position++;
                if (Current is not SemicolonOperator)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Missing semicolon");
                    return null;
                }
                return new DoWhileStatement { Expression = expr, Code = code };
            }
            else if (Current is BracketOpenOperator)
            {
                Position++;
                var statements = new List<Semantic>();
                while (Current is not BracketClosedOperator)
                    if (Current is null)
                        Console.WriteLine($"({Current.Line},{Current.Collumn}) Expected matching closed bracket operator");
                    else
                        statements.Add(Parse());
                Position++;
                return new GroupStatement(statements);
            }
            else if (Current is SemicolonOperator)
            {
                Position++;
                return new EmptyStatement();
            }
            else if (Current is ForKeyword)
            {
                Position++;
                if (Current is not ParenthesisOpenOperator)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Expected an open parenthesis");
                    return null;
                }
                Position++;
                var decl = ParseDeclaration();
                Position++;
                var expr = ParseLOrs();
                if (Current is not SemicolonOperator)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Expected a semicolon");
                    return null;
                }
                Position++;
                var expr2 = ParseLOrs();
                if (Current is not ParenthesisClosedOperator)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Expected a closing parenthesis in for statement");
                    return null;
                }
                Position++;
                var code = ParseStatement();
                return new ForStatement
                {
                    Declaration = decl as Declaration,
                    FirstExpression = expr,
                    SecondExpression = expr2,
                    Statement = code as Statement
                };
            }
            else if (Current is VarKeyword || Current is ConstKeyword)
            {
                var decl = ParseDeclaration();
                if (Current is not SemicolonOperator)
                {
                    Console.WriteLine("Expected a semicolon");
                    return null;
                }
                Position++;
                return decl;
            }
            else
            {
                var expr = ParseLOrs();
                if (Current is not SemicolonOperator)
                {
                    if (Current is null)
                    {
                        Console.WriteLine("Premature EOL when parsing semicolon (no line number or collumn available)");
                        return null;
                    }
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Expected a semicolon");
                    return null;
                }
                Position++;
                return expr;
            }
        }

        private Semantic ParseDeclaration()
        {
            if (Current is VarKeyword)
            {
                Position++;
                var type = ParseType();
                if (Current is not Identifier)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Expected an identifier");
                    return null;
                }
                var ident = Current as Identifier;

                Position++;
                ASTNode ast_value = null;
                if (Current is not EqualOperator && Current is SemicolonOperator)
                    goto SkipAssign;
                else if (Current is not SemicolonOperator && Current is not EqualOperator)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Expected an equal or a semicolon");
                    return null;
                }
                Position++;

                ast_value = ParseLOrs();
            SkipAssign:
                if (Current is not SemicolonOperator)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Expected a semicolon");
                    return null;
                }
                return new VariableDecl { Identifier = ident.Representation, InitVal = ast_value, Type = type as ModernType };
            }
            else if (Current is ConstKeyword)
            {
                Position++;
                var type = ParseType();
                if (Current is not Identifier)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Expected an identifier");
                    return null;
                }
                var ident = Current as Identifier;
                Position++;
                if (Current is not EqualOperator)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Expected an equal");
                    return null;
                }
                Position++;

                var ast_value = ParseLOrs();
                if (Current is not SemicolonOperator)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Expected a semicolon");
                    return null;
                }
                return new ConstantDecl { Identifier = ident.Representation, InitVal = ast_value, Type = type as ModernType };
            }
            else
                return null;
        }

        private Semantic ParseType()
        {
            ModernType type;
            if (Current is SByteKeyword)
                type = new ModernType { Kind = ModernTypeKind.SByte, ChildType = null };
            else if (Current is ByteKeyword)
                type = new ModernType { Kind = ModernTypeKind.Byte, ChildType = null };
            else if (Current is ShortKeyword)
                type = new ModernType { Kind = ModernTypeKind.Short, ChildType = null };
            else if (Current is UShortKeyword)
                type = new ModernType { Kind = ModernTypeKind.UShort, ChildType = null };
            else if (Current is IntKeyword)
                type = new ModernType { Kind = ModernTypeKind.Int, ChildType = null };
            else if (Current is UIntKeyword)
                type = new ModernType { Kind = ModernTypeKind.UInt, ChildType = null };
            else if (Current is LongKeyword)
                type = new ModernType { Kind = ModernTypeKind.Long, ChildType = null };
            else if (Current is ULongKeyword)
                type = new ModernType { Kind = ModernTypeKind.ULong, ChildType = null };
            else if (Current is Least32Keyword)
                type = new ModernType { Kind = ModernTypeKind.Least32, ChildType = null };
            else if (Current is ULeast32Keyword)
                type = new ModernType { Kind = ModernTypeKind.ULeast32, ChildType = null };
            else if (Current is SingleKeyword)
                type = new ModernType { Kind = ModernTypeKind.Single, ChildType = null };
            else if (Current is DoubleKeyword)
                type = new ModernType { Kind = ModernTypeKind.Double, ChildType = null };
            else if (Current is QuadKeyword)
                type = new ModernType { Kind = ModernTypeKind.Quad, ChildType = null };
            else if (Current is VoidKeyword)
                type = new ModernType { Kind = ModernTypeKind.Void, ChildType = null };
            else if (Current is AtOperator)
            {
                Position++;
                type = new ModernType { Kind = ModernTypeKind.Pointer, ChildType = ParseType() as ModernType };
            }
            else if (Current is SquareBracketOpenOperator)
            {
                Position++;
                var expr = ParseLOrs();
                if (Current is not SquareBracketClosedOperator)
                {
                    Console.WriteLine($"({Current.Line},{Current.Collumn}) Missing closing square bracket in array declaration");
                    return null;
                }
                Position++;
                type = new ModernType
                {
                    Kind = ModernTypeKind.Array,
                    ChildType = ParseType() as ModernType,
                    Optional = expr
                };
            }
            else
            {
                Console.WriteLine($"({Current.Line},{Current.Collumn}) Unknown type token {Current.Representation}");
                return null;
            }
            if (type.Kind != ModernTypeKind.Pointer && type.Kind != ModernTypeKind.Array)
                Position++;
            return type;
        }

        public Semantic Parse()
        {
            var semantic = ParseStatement();
            return semantic;
        }
    }
}
