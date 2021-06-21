using ModernSuite.Library.CodeAnalysis.Parsing.AST.Declarations;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Operations;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Statements;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Keywords;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Literals;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Operators;
using ModernSuite.Library.CodeAnalysis.Parsing.Scoping;
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
        public List<Scope> Scopes { get; } = new List<Scope>();
        public int ScopePosition { get; private set; } = 0;
        public Scope CurrentScope => ScopePosition > -1 ? Scopes[ScopePosition] : null;
        private void PushScope()
        {
            Scopes.Add(new Scope { Parent = CurrentScope });
            ScopePosition++;
        }

        private void PopScope()
            => ScopePosition--;

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
                DiagnosticHandler.Add("There is no more tokens left in the buffer.", DiagnosticKind.Error);
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
            else if (token is PlusPlusOperator)
            {
                Position++;
                return new PrefixIncrementOperation { Child = ParsePrimary() };
            }
            else if (token is MinusMinusOperator)
            {
                Position++;
                return new PrefixDecrementOperation { Child = ParsePrimary() };
            }
            else if (token is TildaOperator)
            {
                Position++;
                return new BNotOperation { Child = ParsePrimary() };
            }
            else if (token is ParenthesisOpenOperator)
            {
                Position++;
                var tree = ParseExpression();
                if (Position >= Lexables.Count)
                {
                    DiagnosticHandler.Add($"({token.Line},{token.Collumn}) Missing closing parenthesis (premature EOL).", DiagnosticKind.Error);
                    return null;
                }
                if (Current is ParenthesisClosedOperator)
                {
                    Position++;
                    return new ParenthesizedOperation { Child = tree };
                }
                else
                {
                    DiagnosticHandler.Add($"({token.Line},{token.Collumn}) Missing closing parenthesis.", DiagnosticKind.Error);
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
                var type = ParseType() as ModernType;
                if (type.Kind == ModernTypeKind.Void)
                {
                    // ... No
                    DiagnosticHandler.Add($"({token.Line},{token.Collumn}) void is incomplete type, therefore you cannot get it's size", DiagnosticKind.Error);
                    return null;
                }
                return new SizeofOperation { ToMeasure = type };
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
                        parameters.Add(ParseExpression());
                        if (Current is not CommaOperator)
                            break;
                        Position++;
                    }
                    Position++;
                    if (!CurrentScope.TryLookup(i.Representation, out var _))
                        DiagnosticHandler.Add($"({token.Line},{token.Collumn}) {i.Representation} is not declared", DiagnosticKind.Warn);
                    return new FunctionCallOperation(parameters) { FuncName = i.Representation };
                }
                Position++;
                if (!CurrentScope.TryLookup(i.Representation, out var _))
                    DiagnosticHandler.Add($"({token.Line},{token.Collumn}) {i.Representation} is not declared", DiagnosticKind.Warn);
                return new IdentifierOperation { IdentName = i.Representation };
            }
            else
            {
                DiagnosticHandler.Add($"Token at ({token.Line},{token.Collumn}) is not a valid literal value!", DiagnosticKind.Error);
                return null;
            }
        }

        private ASTNode ParseUnaryPostfix()
        {
            var operand = ParsePrimary();
            if (Position >= Lexables.Count)
                return operand;

            while (Current is PlusPlusOperator || Current is MinusMinusOperator)
            {
                var op = Current;
                if (op is not Operator)
                {
                    DiagnosticHandler.Add($"Expected an operator at ({Current.Line},{Current.Collumn})", DiagnosticKind.Error);
                    return null;
                }
                Position++;
                operand = op is PlusPlusOperator ? new PostfixIncrementOperation { Child = operand } :
                    op is MinusMinusOperator ? new PostfixDecrementOperation { Child = operand } :
                    null;
            }
            return operand;
        }

        private ASTNode ParseFactor()
        {
            var left = ParseUnaryPostfix();
            if (Position >= Lexables.Count)
            {
                return left;
            }

            while (Current is FactorialOperator)
            {
                var op = Current;
                if (op is not Operator)
                {
                    DiagnosticHandler.Add($"Expected an operator at ({Current.Line},{Current.Collumn})", DiagnosticKind.Error);
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
                    DiagnosticHandler.Add($"Expected an operator at ({Current.Line},{Current.Collumn})", DiagnosticKind.Error);
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
                    DiagnosticHandler.Add($"Expected an operator at ({Current.Line},{Current.Collumn})", DiagnosticKind.Error);
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
                    DiagnosticHandler.Add($"Expected an operator at ({Current.Line},{Current.Collumn})", DiagnosticKind.Error);
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
                    DiagnosticHandler.Add($"Expected an operator at ({Current.Line},{Current.Collumn})", DiagnosticKind.Error);
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
                    DiagnosticHandler.Add($"Expected an operator at ({Current.Line},{Current.Collumn})", DiagnosticKind.Error);
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
                    DiagnosticHandler.Add($"Expected an operator at ({Current.Line},{Current.Collumn})", DiagnosticKind.Error);
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
                    DiagnosticHandler.Add($"Expected an operator at ({Current.Line},{Current.Collumn})", DiagnosticKind.Error);
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
                    DiagnosticHandler.Add($"Expected an operator at ({Current.Line},{Current.Collumn})", DiagnosticKind.Error);
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
                    DiagnosticHandler.Add($"Expected an operator at ({Current.Line},{Current.Collumn})", DiagnosticKind.Error);
                    return null;
                }
                Position++;
                var right = ParseLOrs();
                left = new LOrOperation { Left = left, Right = right };
            }
            return left;
        }

        private ASTNode ParseCast()
        {
            var left = ParseLOrs();
            if (Position >= Lexables.Count)
            {
                return left;
            }
            if (Current is LargeArrowOperator)
            {
                Position++;
                var right = ParseType();
                left = new CastOperation { Child = left, Type = right as ModernType };
            }
            return left;
        }

        private ASTNode ParseConditional()
        {
            var cond = ParseCast();
            if (Position >= Lexables.Count)
            {
                return cond;
            }
            while (Current is WhatOperator)
            {
                if (Position >= Lexables.Count)
                {
                    return cond;
                }
                var op = Current;
                if (op is not Operator)
                {
                    DiagnosticHandler.Add($"Expected an operator at ({Current.Line},{Current.Collumn})", DiagnosticKind.Error);
                    return null;
                }
                Position++;
                var left = ParseConditional();
                if (Current is not ColonOperator)
                {
                    if (Current is null)
                    {
                        DiagnosticHandler.Add($"Premature EOL after true expression in conditional expression (no source code position available)", DiagnosticKind.Error);
                    }
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected a colon after true expression in conditionnal expression", DiagnosticKind.Error);
                    return null;
                }
                Position++;
                var right = ParseConditional();
                cond = new ConditionalOperation { Condition = cond, IfTrue = left, IfFalse = right };
            }
            return cond;
        }

        private ASTNode ParseAssign()
        {
            var left = ParseConditional();
            if (Position >= Lexables.Count)
            {
                return left;
            }
            while (Current is EqualOperator ||
                Current is PlusEqualOperator ||
                Current is MinusEqualOperator ||
                Current is StarEqualOperator ||
                Current is SlashEqualOperator ||
                Current is PercentEqualOperator ||
                Current is LeftArrowsEqualOperator ||
                Current is RightArrowsEqualOperator ||
                Current is AmpersandEqualOperator ||
                Current is PipeEqualOperator ||
                Current is CaretEqualOperator)
            {
                if (Position >= Lexables.Count)
                {
                    return left;
                }
                var op = Current;
                if (op is not Operator)
                {
                    DiagnosticHandler.Add($"Expected an operator at ({Current.Line},{Current.Collumn})", DiagnosticKind.Error);
                    return null;
                }
                Position++;
                var right = ParseAssign();
                left = op is EqualOperator ? new AssignmentOperation { Left = left, Right = right } :
                    op is PlusEqualOperator ? new IncrementOperation { Left = left, Right = right } :
                    op is MinusEqualOperator ? new DecrementOperation { Left = left, Right = right } :
                    op is StarEqualOperator ? new MultiplyAssignOperation { Left = left, Right = right } :
                    op is SlashEqualOperator ? new DivideAssignOperation { Left = left, Right = right } :
                    op is PercentEqualOperator ? new RemainderAssignOperation { Left = left, Right = right } :
                    op is LeftArrowsEqualOperator ? new BLSAssignOperation { Left = left, Right = right } :
                    op is RightArrowsEqualOperator ? new BRSAssignOperation { Left = left, Right = right } :
                    op is AmpersandEqualOperator ? new AndAssignOperation { Left = left, Right = right } :
                    op is CaretEqualOperator ? new XorAssignOperation { Left = left, Right = right } :
                    new OrAssignOperation { Left = left, Right = right };
            }
            return left;
        }

        private ASTNode ParseExpression()
            => ParseAssign();

        private Semantic ParseStatement()
        {
            if (Current is not FunctionKeyword && Current is not VarKeyword && Current is not ConstKeyword &&
                !CurrentScope.IsFunction)
            {
                DiagnosticHandler.Add($"A statement or expression may only be used in a function body", DiagnosticKind.Error);
                return null;
            }
            if (Current is IfKeyword)
            {
                Position++;
                if (Current is ParenthesisOpenOperator)
                {
                    Position++;
                    var expr = ParseExpression();
                    if (Current is not ParenthesisClosedOperator)
                    {
                        DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Missing closing parenthesis in if statement", DiagnosticKind.Error);
                        return null;
                    }
                    Position++;
                    var code = ParseSemantic();
                    Semantic else_code = null;
                    if (Current is ElseKeyword)
                    {
                        Position++;
                        else_code = ParseSemantic();
                    }
                    return new IfElseStatement { TrueCode = code, ElseCode = else_code, Expression = expr };
                }
                DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Invalid if statement syntax", DiagnosticKind.Error);
                return null;
            }
            else if (Current is GotoKeyword)
            {
                Position++;
                var objective = ParseExpression();
                if (objective is null)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Invalid goto syntax", DiagnosticKind.Error);
                    return null;
                }
                if (Current is not SemicolonOperator)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected a semicolon", DiagnosticKind.Error);
                    return null;
                }
                Position++;
                return new GotoStatement { Objective = objective };
            }
            else if (Current is WhileKeyword)
            {
                Position++;
                if (Current is ParenthesisOpenOperator)
                {
                    Position++;
                    var expr = ParseExpression();
                    if (Current is not ParenthesisClosedOperator)
                    {
                        DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Missing closing parenthesis in if statement", DiagnosticKind.Error);
                        return null;
                    }
                    Position++;
                    PushScope();
                    CurrentScope.BreakAllowed = true;
                    CurrentScope.ContinueAllowed = true;
                    var code = ParseSemantic();
                    PopScope();
                    return new WhileStatement { Expression = expr, While = code };
                }
                DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Invalid while statement syntax", DiagnosticKind.Error);
                return null;
            }
            else if (Current is DoKeyword)
            {
                Position++;
                PushScope();
                CurrentScope.BreakAllowed = true;
                CurrentScope.ContinueAllowed = true;
                var code = ParseSemantic();
                PopScope();
                if (Current is not WhileKeyword)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Invalid do...while statement syntax", DiagnosticKind.Error);
                    return null;
                }
                Position++;
                if (Current is not ParenthesisOpenOperator)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Invalid do...while statement syntax", DiagnosticKind.Error);
                    return null;
                }
                Position++;
                var expr = ParseExpression();
                if (Current is not ParenthesisClosedOperator)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Missing closing parenthesis in do...while statement", DiagnosticKind.Error);
                    return null;
                }
                Position++;
                if (Current is not SemicolonOperator)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Missing semicolon", DiagnosticKind.Error);
                    return null;
                }
                return new DoWhileStatement { Expression = expr, Code = code };
            }
            else if (Current is BracketOpenOperator)
            {
                Position++;
                var statements = new List<Semantic>();
                PushScope();
                while (Current is not BracketClosedOperator)
                    if (Current is null)
                        DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected matching closed bracket operator", DiagnosticKind.Error);
                    else
                        statements.Add(ParseSemantic());
                Position++;
                PopScope();
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
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected an open parenthesis", DiagnosticKind.Error);
                    return null;
                }
                Position++;
                PushScope();
                CurrentScope.BreakAllowed = true;
                CurrentScope.ContinueAllowed = true;
                var decl = ParseDeclaration();
                Position++;
                var expr = ParseExpression();
                if (Current is not SemicolonOperator)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected a semicolon", DiagnosticKind.Error);
                    return null;
                }
                Position++;
                var expr2 = ParseExpression();
                if (Current is not ParenthesisClosedOperator)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected a closing parenthesis in for statement", DiagnosticKind.Error);
                    return null;
                }
                Position++;
                var code = ParseStatement();
                PopScope();
                return new ForStatement
                {
                    Declaration = decl as Declaration,
                    FirstExpression = expr,
                    SecondExpression = expr2,
                    Statement = code
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
            else if (Current is FunctionKeyword)
                return ParseDeclaration();
            else if (Current is ReturnKeyword)
            {
                if (!CurrentScope.IsFunction)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) return can only be used in a function", DiagnosticKind.Error);
                    return null;
                }
                Position++;
                var expr = ParseExpression();
                if (Current is not SemicolonOperator)
                {
                    if (Current is null)
                    {
                        DiagnosticHandler.Add("Premature EOL when parsing semicolon (no line number or collumn available)", DiagnosticKind.Error);
                        return null;
                    }
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected a semicolon", DiagnosticKind.Error);
                }
                return new ReturnStatement { Expression = expr };
            }
            else if (Current is BreakKeyword)
            {
                if (!CurrentScope.BreakAllowed)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) break cannot be used in this scope", DiagnosticKind.Error);
                    return null;
                }
                if (Current is not SemicolonOperator)
                {
                    if (Current is null)
                    {
                        DiagnosticHandler.Add("Premature EOL when parsing semicolon (no line number or collumn available)", DiagnosticKind.Error);
                        return null;
                    }
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected a semicolon", DiagnosticKind.Error);
                }
                return new BreakStatement();
            }
            else if (Current is ContinueKeyword)
            {
                if (!CurrentScope.ContinueAllowed)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) continue cannot be used in this scope", DiagnosticKind.Error);
                    return null;
                }
                if (Current is not SemicolonOperator)
                {
                    if (Current is null)
                    {
                        DiagnosticHandler.Add("Premature EOL when parsing semicolon (no line number or collumn available)", DiagnosticKind.Error);
                        return null;
                    }
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected a semicolon", DiagnosticKind.Error);
                }
                return new ContinueStatement();
            }
            else
            {
                var expr = ParseExpression();
                if (Current is not SemicolonOperator)
                {
                    if (Current is null)
                    {
                        DiagnosticHandler.Add("Premature EOL when parsing semicolon (no line number or collumn available)", DiagnosticKind.Error);
                        return null;
                    }
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected a semicolon", DiagnosticKind.Error);
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
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected an identifier", DiagnosticKind.Error);
                    return null;
                }
                var ident = Current as Identifier;

                Position++;
                ASTNode ast_value = null;
                if (Current is not EqualOperator && Current is SemicolonOperator)
                    goto SkipAssign;
                else if (Current is not SemicolonOperator && Current is not EqualOperator)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected an equal or a semicolon", DiagnosticKind.Error);
                    return null;
                }
                Position++;

                ast_value = ParseExpression();
            SkipAssign:
                if (Current is not SemicolonOperator)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected a semicolon", DiagnosticKind.Error);
                    return null;
                }
                var decl = new VariableDecl { Identifier = ident.Representation, InitVal = ast_value, Type = type as ModernType }; ;
                if (!CurrentScope.TryDeclare(decl))
                {
                    DiagnosticHandler.Add($"({ident.Line},{ident.Collumn}) {ident.Representation} was already defined", DiagnosticKind.Error);
                    return null;
                }
                return decl;
            }
            else if (Current is ConstKeyword)
            {
                Position++;
                var type = ParseType();
                if (Current is not Identifier)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected an identifier", DiagnosticKind.Error);
                    return null;
                }
                var ident = Current as Identifier;
                Position++;
                if (Current is not EqualOperator)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected an equal", DiagnosticKind.Error);
                    return null;
                }
                Position++;

                var ast_value = ParseExpression();
                if (Current is not SemicolonOperator)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected a semicolon", DiagnosticKind.Error);
                    return null;
                }
                var decl = new ConstantDecl { Identifier = ident.Representation, InitVal = ast_value, Type = type as ModernType };
                if (!CurrentScope.TryDeclare(decl))
                {
                    DiagnosticHandler.Add($"({ident.Line},{ident.Collumn}) {ident.Representation} was already defined", DiagnosticKind.Error);
                    return null;
                }
                return decl;
            }
            else if (Current is FunctionKeyword)
            {
                Position++;
                var type = ParseType();
                if (Current is not Identifier)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected an identifier", DiagnosticKind.Error);
                    return null;
                }
                var ident = Current;
                Position++;
                if (Current is not ParenthesisOpenOperator)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected an open parenthesis", DiagnosticKind.Error);
                    return null;
                }
                Position++;
                PushScope();
                CurrentScope.IsFunction = true;
                var paramList = new List<Parameter>();
                while (Current is not ParenthesisClosedOperator)
                {
                    if (Current is null)
                    {
                        DiagnosticHandler.Add($"Premature EOL in function declaration", DiagnosticKind.Error);
                        return null;
                    }

                    var paramType = ParseType();
                    if (Current is not Identifier)
                    {
                        DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected an identifier", DiagnosticKind.Error);
                        return null;
                    }
                    var paramIdent = Current as Identifier;
                    Position++;
                    var parameter = new Parameter
                    {
                        Identifier = paramIdent.Representation,
                        Type = paramType as ModernType
                    };
                    paramList.Add(parameter);
                    if (!CurrentScope.TryDeclare(parameter))
                    {
                        DiagnosticHandler.Add($"({paramIdent.Line},{paramIdent.Collumn}) Cannot declare parameter " +
                            $"{paramIdent.Representation} because it is already declared in a higher scope.",
                            DiagnosticKind.Error);
                        return null;
                    }
                    if (Current is not CommaOperator && Current is not ParenthesisClosedOperator)
                    {
                        DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Expected ',' or ')'", DiagnosticKind.Error);
                        return null;
                    }
                    if (Current is CommaOperator)
                        Position++;
                }
                Position++;
                var code = ParseSemantic();
                if (code is not ASTNode && code is not GroupStatement)
                {
                    DiagnosticHandler.Add($"Expected a group statement (e.g. {{}}) or an expression", DiagnosticKind.Error);
                    return null;
                }
                PopScope();
                var func = new FuncDecl
                {
                    Code = code,
                    Identifier = ident.Representation,
                    Type = type as ModernType,
                    Parameters = paramList
                };
                if (!CurrentScope.TryDeclare(func))
                {
                    DiagnosticHandler.Add($"Cannot declare function. Symbol {func.Identifier} is already declared", DiagnosticKind.Error);
                    return null;
                }
                return func;
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
            else if (Current is StringKeyword)
                type = new ModernType { Kind = ModernTypeKind.String, ChildType = null };
            else if (Current is AtOperator)
            {
                Position++;
                type = new ModernType { Kind = ModernTypeKind.Pointer, ChildType = ParseType() as ModernType };
            }
            else if (Current is SquareBracketOpenOperator)
            {
                Position++;
                var expr = ParseExpression();
                if (Current is not SquareBracketClosedOperator)
                {
                    DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Missing closing square bracket in array declaration", DiagnosticKind.Error);
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
                DiagnosticHandler.Add($"({Current.Line},{Current.Collumn}) Unknown type token {Current.Representation}", DiagnosticKind.Error);
                return null;
            }
            if (type.Kind != ModernTypeKind.Pointer && type.Kind != ModernTypeKind.Array)
                Position++;
            return type;
        }

        private Semantic ParseSemantic()
        {
            if (Current is null)
                return null;
            var semantic = ParseStatement();
            return semantic;
        }

        public ModernProgram Parse()
        {
            var semantics = new List<Semantic>();

            Scopes.Add(new Scope());
            // scope position is already 0

            while (true)
            {
                var semantic = ParseSemantic();
                if (semantic is null)
                    break;
                semantics.Add(semantic);
            }

            return new ModernProgram(semantics);
        }
    }
}
