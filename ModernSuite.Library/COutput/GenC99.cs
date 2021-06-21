using ModernSuite.Library.CodeAnalysis;
using ModernSuite.Library.CodeAnalysis.Parsing.AST;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Declarations;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Operations;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Statements;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Keywords;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Literals;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.COutput
{
    public sealed class GenC99
    {
        private string ParseContainingCharacters(string str)
        {
            var builder = new StringBuilder();
            foreach (var c in str)
                switch (c)
                {
                case '\0':
                    builder.Append(@"\0");
                    break;
                case '"':
                    builder.Append(@"\""");
                    break;
                case '\'':
                    builder.Append(@"\'");
                    break;
                case '\\':
                    builder.Append(@"\\");
                    break;
                case '\t':
                    builder.Append(@"\t");
                    break;
                case '\v':
                    builder.Append(@"\v");
                    break;
                case '\n':
                    builder.Append(@"\n");
                    break;
                case '\r':
                    builder.Append(@"\r");
                    break;
                case '\f':
                    builder.Append(@"\f");
                    break;
                case '\b':
                    builder.Append(@"\b");
                    break;
                case '\a':
                    builder.Append(@"\a");
                    break;
                default:
                    builder.Append(c);
                    break;
                }
            return builder.ToString();
        }
        private string ParseExpression(ASTNode node)
        {
            if (node is LiteralASTNode lan)
            {
                if (lan.Lexable is StringLiteral)
                    return $"\"{ParseContainingCharacters(lan.Lexable.Representation)}\"";
                if (lan.Lexable is FloatingLiteral fl)
                    return $"{((decimal)fl.Value).ToString(CultureInfo.InvariantCulture)}";
                return $"{lan.Lexable.Representation}";
            }
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
            else if (node is PrefixIncrementOperation pio)
                return $"++{ParseExpression(pio.Child)}";
            else if (node is PrefixDecrementOperation pdo)
                return $"--{ParseExpression(pdo.Child)}";
            else if (node is PostfixIncrementOperation poio)
                return $"{ParseExpression(poio.Child)}++";
            else if (node is PostfixDecrementOperation podo)
                return $"{ParseExpression(podo.Child)}--";
            else if (node is CastOperation co)
                return $"({ParseType(co.Type)})({ParseExpression(co.Child)})";
            else if (node is AssignmentOperation aso)
                return $"{ParseExpression(aso.Left)}={ParseExpression(aso.Right)}";
            else if (node is IncrementOperation ico)
                return $"{ParseExpression(ico.Left)}+={ParseExpression(ico.Right)}";
            else if (node is DecrementOperation dco)
                return $"{ParseExpression(dco.Left)}-={ParseExpression(dco.Right)}";
            else if (node is MultiplyAssignOperation mao)
                return $"{ParseExpression(mao.Left)}*={ParseExpression(mao.Right)}";
            else if (node is DivideAssignOperation dao)
                return $"{ParseExpression(dao.Left)}/={ParseExpression(dao.Right)}";
            else if (node is RemainderAssignOperation rao)
                return $"{ParseExpression(rao.Left)}/={ParseExpression(rao.Right)}";
            else if (node is AndAssignOperation aao)
                return $"{ParseExpression(aao.Left)}&={ParseExpression(aao.Right)}";
            else if (node is XorAssignOperation xao)
                return $"{ParseExpression(xao.Left)}^={ParseExpression(xao.Right)}";
            else if (node is OrAssignOperation oao)
                return $"{ParseExpression(oao.Left)}|={ParseExpression(oao.Right)}";
            else if (node is BRSAssignOperation brsao)
                return $"{ParseExpression(brsao.Left)}>>={ParseExpression(brsao.Right)}";
            else if (node is BRSAssignOperation blsao)
                return $"{ParseExpression(blsao.Left)}<<={ParseExpression(blsao.Right)}";
            else if (node is ConditionalOperation cdo)
                return $"{ParseExpression(cdo.Condition)}?{ParseExpression(cdo.IfTrue)}:{ParseExpression(cdo.IfFalse)}";
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
                    ModernTypeKind.String => "sizeof(void *)",  // sizeof(char *) is the same as sizeof(void *)
                    _ => throw new Exception("Missing Modern Type implementation")
                };
            else
                return "";
        }

        private string ParseType(ModernType type)
        {
            if (type is null)
            {
                DiagnosticHandler.Add("Invalid type", DiagnosticKind.Error);
                return "";
            }
            if (type.Kind == ModernTypeKind.Pointer && type.ChildType.Kind == ModernTypeKind.Void)
            {
                DiagnosticHandler.Add($"Usage of anonymous pointer", DiagnosticKind.Info);
            }
            if (type.Kind == ModernTypeKind.Pointer && type.ChildType.Kind == ModernTypeKind.String)
                return "char*";
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
                ModernTypeKind.String => "const char*",
            };
        }

        public string ParseStatements(Semantic semantic)
        {
            if (semantic is ModernProgram mp)
            {
                var str = "";
                foreach (var child in mp.GetSemantics())
                    str += ParseStatements(child);
                return str;
            }
            else if (semantic is IfElseStatement ies)
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
            else if (semantic is ReturnStatement rs)
                return $"return {ParseExpression(rs.Expression)};";
            else if (semantic is BreakStatement)
                return "break;";
            else if (semantic is ContinueStatement)
                return "continue;";                 // TODO: Continue in switch statements
            else if (semantic is GroupStatement grs)
            {
                var output = "";
                foreach (var statement in grs.SubSemantics)
                    output += ParseStatements(statement);
                return $"{{{output}}}";
            }
            else if (semantic is WhileStatement ws)
                return $"while({ParseExpression(ws.Expression)}){ParseStatements(ws.While)}";
            else if (semantic is EnumDecl ed)
                return "";
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
            else if (semantic is FuncDecl fd)
            {
                var type = ParseType(fd.Type);
                if (type.StartsWith('?'))
                {
                    DiagnosticHandler.Add($"({fd.Identifier}) A function cannot return an array", DiagnosticKind.Error);
                    return null;
                }
                var parameters = "";
                foreach (var paramType in fd.Parameters)
                {
                    var paramTypeAsStr = ParseType(paramType.Type);
                    if (paramTypeAsStr.StartsWith('?'))
                    {
                        DiagnosticHandler.Add($"({fd.Identifier}) A function parameter cannot be of type array", DiagnosticKind.Error);
                        return null;
                    }
                    parameters += $"{paramTypeAsStr} {paramType.Identifier},";
                }
                parameters = parameters.TrimEnd(',');
                var code = "";
                if (fd.Code is ASTNode astnode)
                    code = $"{{return {ParseExpression(astnode)};}}";
                else
                    code = ParseStatements(fd.Code);
                return $"{type} {fd.Identifier}({parameters}){code}";
            }
            else if (semantic is ForStatement fs)
                return $"for({ParseStatements(fs.Declaration)}{ParseExpression(fs.FirstExpression)};" +
                    $"{ParseExpression(fs.SecondExpression)}){ParseStatements(fs.Statement)}";
            else
                return "";
        }
    }
}
