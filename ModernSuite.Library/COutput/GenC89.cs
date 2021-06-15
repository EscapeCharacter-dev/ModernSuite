using ModernSuite.Library.CodeAnalysis.Parsing.AST;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Operations;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.COutput
{
    public sealed class GenC89
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
            else
                return "";
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
            else
                return "";
        }
    }
}
