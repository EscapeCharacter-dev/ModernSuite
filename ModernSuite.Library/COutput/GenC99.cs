using ModernSuite.Library.CodeAnalysis.Parsing.AST;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Declarations;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Operations;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Statements;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Keywords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.COutput
{
    public sealed class GenC99
    {
        private string ParseExpression(ASTNode node)
        {
            if (node is LiteralASTNode lan)
                return $"{lan.Lexable.Representation}";
            else if (node is AdditionOperation ao)
                return $"{ParseExpression(ao.Left)}+{ParseExpression(ao.Right)}";
            else if (node is SubtractionOperation so)
                return $"{ParseExpression(so.Left)}-{ParseExpression(so.Right)}";
            else if (node is MultiplicationOperation mo)
                return $"{ParseExpression(mo.Left)}*{ParseExpression(mo.Right)}";
            else if (node is DivisionOperation dvo)
                return $"{ParseExpression(dvo.Left)}/{ParseExpression(dvo.Right)}";
            else if (node is RemainderOperation ro)
                return $"{ParseExpression(ro.Left)}%{ParseExpression(ro.Right)}";
            else if (node is LAndOperation lao)
                return $"{ParseExpression(lao.Left)}&&{ParseExpression(lao.Right)}";
            else if (node is BAndOperation bao)
                return $"{ParseExpression(bao.Left)}&{ParseExpression(bao.Right)}";
            else if (node is LOrOperation loo)
                return $"{ParseExpression(loo.Left)}||{ParseExpression(loo.Right)}";
            else if (node is BOrOperation boo)
                return $"{ParseExpression(boo.Left)}|{ParseExpression(boo.Right)}";
            else if (node is NegativeOperation no)
                return $"-{ParseExpression(no.Child)}";
            else if (node is UnaryPlusOperation upo)
                return $"+{ParseExpression(upo.Child)}";
            else if (node is ParenthesizedOperation po)
                return $"({ParseExpression(po.Child)})";
            else if (node is LNotOperation lno)
                return $"!{ParseExpression(lno.Child)}";
            else if (node is BNotOperation bno)
                return $"~{ParseExpression(bno.Child)}";
            else if (node is BinaryLeftShiftOperation blso)
                return $"{ParseExpression(blso.Left)}<<{ParseExpression(blso.Right)}";
            else if (node is BinaryRightShiftOperation brso)
                return $"{ParseExpression(brso.Left)}>>{ParseExpression(brso.Right)}";
            else if (node is LowerOperation lo)
                return $"{ParseExpression(lo.Left)}<{ParseExpression(lo.Right)}";
            else if (node is LowerEqualOperation leo)
                return $"{ParseExpression(leo.Left)}<={ParseExpression(leo.Right)}";
            else if (node is GreaterOperation go)
                return $"{ParseExpression(go.Left)}>{ParseExpression(go.Right)}";
            else if (node is GreaterEqualOperation geo)
                return $"{ParseExpression(geo.Left)}>={ParseExpression(geo.Right)}";
            else if (node is EqualityOperation eo)
                return $"{ParseExpression(eo.Left)}=={ParseExpression(eo.Right)}";
            else if (node is NotEqualOperation neo)
                return $"{ParseExpression(neo.Left)}!={ParseExpression(neo.Right)}";
            else if (node is BXorOperation bxo)
                return $"{ParseExpression(bxo.Left)}^{ParseExpression(bxo.Right)}";
            else if (node is AddressOfOperation aoo)
                return $"&{ParseExpression(aoo.Child)}";
            else if (node is ValueOfOperation voo)
                return $"*{ParseExpression(voo.Child)}";
            else if (node is FunctionCallOperation fco)
            {
                var operands = "";
                foreach (var child in fco.Children)
                    operands += $"{ParseExpression(child)},";
                operands = operands.Trim(',');
                return $"{fco.FuncName}({operands})";
            }
            else if (node is IdentifierOperation io)
                return $"{io.IdentName}";
            else if (node is SizeofOperation szo)
                return szo.ToMeasure.Kind switch
                {
                    ModernTypeKind.Byte => "1",     // C standard says sizeof(char) = 1
                    ModernTypeKind.SByte => "1",    // C standard says sizeof(char) = 1
                    ModernTypeKind.Void => "0",     // void has no size
                    ModernTypeKind.Short => "sizeof(short)",
                    ModernTypeKind.UShort => "sizeof(short)",
                    ModernTypeKind.Int => "sizeof(int)",
                    ModernTypeKind.UInt => "sizeof(int)",
                    ModernTypeKind.Least32 => "sizeof(long)",
                    ModernTypeKind.ULeast32 => "sizeof(long)",
                    ModernTypeKind.Long => "sizeof(long long)",
                    ModernTypeKind.ULong => "sizeof(long long)",
                    ModernTypeKind.Single => "sizeof(float)",
                    ModernTypeKind.Double => "sizeof(double)",
                    ModernTypeKind.Quad => "sizeof(long double)",
                    ModernTypeKind.Pointer => "sizeof(void *)",
                    ModernTypeKind.Array => "sizeof(void *)",
                    ModernTypeKind.Function => "sizeof(void *)",
                    _ => throw new Exception("Missing Modern Type implementation")
                };
            else
                return "";
        }

        private string ParseType(ModernType type)
        {
            return type.Kind switch
            {
                ModernTypeKind.Byte => "char",
                ModernTypeKind.SByte => "signed char",
                ModernTypeKind.Short => "short",
                ModernTypeKind.UShort => "unsigned short",
                ModernTypeKind.Int => "int",
                ModernTypeKind.UInt => "unsigned int",
                ModernTypeKind.Least32 => "long",
                ModernTypeKind.ULeast32 => "unsigned long",
                ModernTypeKind.Long => "long long",
                ModernTypeKind.ULong => "unsigned long long",
                ModernTypeKind.Void => "void",
                ModernTypeKind.Single => "float",
                ModernTypeKind.Double => "double",
                ModernTypeKind.Quad => "long double",
                ModernTypeKind.Pointer => $"{ParseType(type.ChildType)}*",
                ModernTypeKind.Array => $"?{ParseType(type.ChildType)}",
            };
        }

        public string ParseStatements(Semantic semantic)
        {
            if (semantic is IfElseStatement ies)
                return $"if({ParseExpression(ies.Expression)}){ParseStatements(ies.TrueCode)}" +
                    $"{(ies.ElseCode != null ? "else " + ParseStatements(ies.ElseCode) : "")}";
            else if (semantic is GotoStatement gs)
            {
                if (gs.Objective is IdentifierOperation)
                    return $"goto {ParseExpression(gs.Objective)};";
                else
                    return $"__voidptr_storage = {ParseExpression(gs.Objective)};goto *__voidptr_storage;";
            }
            else if (semantic is ASTNode an)
                return $"{ParseExpression(an)};";
            else if (semantic is GroupStatement grs)
            {
                var output = "";
                foreach (var statement in grs.SubSemantics)
                    output += ParseStatements(statement);
                return $"{{{output}}}";
            }
            else if (semantic is WhileStatement ws)
                return $"while({ParseExpression(ws.Expression)}){ParseStatements(ws.While)}";
            else if (semantic is DoWhileStatement dws)
                return $"do {ParseStatements(dws.Code)}while({ParseExpression(dws.Expression)});";
            else if (semantic is VariableDecl vdcl)
            {
                var type = ParseType(vdcl.Type);
                var isArray = type.StartsWith('?') ? true : false;
                type = type.TrimStart('?');
                return
$"{type} {vdcl.Identifier}{(isArray ? $"[{ParseExpression(vdcl.Type.Optional)}]" : "")}={ParseExpression(vdcl.InitVal)};";
            }

            else if (semantic is ConstantDecl cd)
            {
                var type = ParseType(cd.Type);
                var isArray = type.StartsWith('?') ? true : false;
                type = type.TrimStart('?');
                return
$"const {type} {cd.Identifier}{(isArray ? $"[{ParseExpression(cd.Type.Optional)}]" : "")}={ParseExpression(cd.InitVal)};";
            }

            else if (semantic is ForStatement fs)
                return $"for({ParseStatements(fs.Declaration)}{ParseExpression(fs.FirstExpression)};" +
                    $"{ParseExpression(fs.SecondExpression)}){ParseStatements(fs.Statement)}";
            else if (semantic is ManagedStatement ms)
                return $"{{{ParseStatements(ms.Decl)}{ParseStatements(ms.Statement)}_managedfree({ms.Decl.Identifier});}}";
            else
                return "";
        }
    }
}
