using ModernSuite.Library.CodeAnalysis.Parsing.AST;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Declarations;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.Scoping
{
    public sealed class ScopeResolver
    {
        public ScopeResolver(ModernProgram program)
            => _program = program;

        private readonly ModernProgram _program;
        private Semantic ScopeSingleSemantic(Semantic semantic, Scope parent)
        {
            semantic.Scope = new Scope { Parent = parent };
            if (semantic is Declaration d)
                parent.TryDeclare(d);
            else if (semantic is DoWhileStatement dws)
            {
                dws.Code = ScopeSingleSemantic(dws.Code, semantic.Scope);
                semantic = dws;
            }
            else if (semantic is ForStatement fs)
            {
                // Types are preserved by the copy, so we can do anonymous casts
                fs.Statement = ScopeSingleSemantic(fs.Statement, semantic.Scope) as Statement;
                semantic.Scope.TryDeclare(fs.Declaration);
                semantic = fs;
            }
            else if (semantic is GroupStatement gs)
            {
                var _semantics = gs.SubSemantics;
                var semantics = new List<Semantic>();
                foreach (var child in _semantics)
                    semantics.Add(ScopeSingleSemantic(child, semantic.Scope));
                semantic = new GroupStatement(semantics);
            }
            else if (semantic is IfElseStatement ies)
            {
                ies.TrueCode = ScopeSingleSemantic(ies.TrueCode, semantic.Scope);
                ies.ElseCode = ScopeSingleSemantic(ies.ElseCode, semantic.Scope);
                semantic = ies;
            }
            else if (semantic is WhileStatement ws)
            {
                ws.While = ScopeSingleSemantic(ws.While, semantic.Scope);
                semantic = ws;
            }
            else if (semantic is ManagedStatement ms)
            {
                semantic.Scope.TryDeclare(ms.Decl);
                ms.Statement = ScopeSingleSemantic(ms.Statement, semantic.Scope);
                semantic = ms;
            }
            return semantic;
        }

        public ModernProgram ReturnScopedProgram()
        {
            var program = _program;
            program.Scope = new Scope();

            var modifiable = _program.GetSemantics();
            var result = new List<Semantic>();

            foreach (var semantic in modifiable)
                result.Add(ScopeSingleSemantic(semantic, program.Scope));

            program.SetSemantics(result);

            return program;
        }
    }
}
