using ModernSuite.Library.CodeAnalysis.Parsing.AST;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Operations;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Statements;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Literals;
using ModernSuite.Library.COutput;
using ModernSuite.Library.Xml;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace ModernSuite.Library.CodeAnalysis
{
    public class Test
    {
        public void ParseTest()
        {
            Parser parser;
            ModernProgram semantic;
            string ccode = "static void *__voidptr_storage;\n";
            while (true)
            {
                DiagnosticHandler.Clear();
                Console.Write($"{Environment.CurrentDirectory}> ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                parser = new Parser(File.ReadAllText(Console.ReadLine()));
                Console.ResetColor();
                semantic = parser.Parse();
                ccode = "static void *__voidptr_storage;\n";
                var should = DiagnosticHandler.Display();
                if (!should)
                {
                    DiagnosticHandler.Clear();
                    ccode += new GenC99().ParseStatements(semantic);
 
                    var nshould = DiagnosticHandler.Display();
                    if (!should || !nshould)
                        Console.WriteLine(ccode);
                }
            }
        }

        private object Evaluate(Semantic node)
        {
            if (node is null)
                return 0;
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
            else if (node is AddressOfOperation aoo)
                return GCHandle.Alloc(Evaluate(aoo.Child), GCHandleType.Pinned).AddrOfPinnedObject().ToInt64();
            else if (node is ValueOfOperation voo)
                return Marshal.ReadInt64(new IntPtr(Convert.ToInt64(Evaluate(voo.Child))));
            else if (node is FunctionCallOperation fco)
            {
                if (fco.FuncName == "test")
                {
                    var total = 0L;
                    foreach (var param in fco.Children)
                        total += Convert.ToInt64(Evaluate(param));
                    return total;
                }
                return 0;
            }
            else if (node is IdentifierOperation io)
                return 0;
            else if (node is IfElseStatement ies)
                return Convert.ToInt64(Evaluate(ies.Expression)) != 0 ? Evaluate(ies.TrueCode) : Evaluate(ies.ElseCode);
            else if (node is GotoStatement gs)
                return 0xFF;
            else if (node is GroupStatement grs)
            {
                foreach (var statement in grs.SubSemantics)
                    Evaluate(statement);
                return 0xFE;
            }
            else
                return null;
        }
    }
}
