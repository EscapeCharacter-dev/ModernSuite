using ModernSuite.Library.CodeAnalysis.Parsing.AST;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.Scoping
{
    public sealed class Scope
    {
        public Dictionary<string, Declaration> Symbols { get; init; } = new Dictionary<string, Declaration>();

        public Scope Parent { get; init; }
        private bool _isFunction = false;
        private bool _breakAllowed = false;
        private bool _continueAllowed = false;
        
        public bool IsFunction
        {
            get => _isFunction ? true : Parent != null ? Parent.IsFunction : false;
            set => _isFunction = value;
        }

        public bool BreakAllowed
        {
            get => _breakAllowed ? true : Parent != null ? Parent.BreakAllowed : false;
            set => _breakAllowed = value;
        }

        public bool ContinueAllowed
        {
            get => _continueAllowed ? true : Parent != null ? Parent.ContinueAllowed : false;
            set => _continueAllowed = value;
        }

        public bool TryDeclare(Declaration declaration)
        {
            if (Symbols.ContainsKey(declaration.Identifier) || Parent != null && Parent.TryLookup(declaration.Identifier, out var _))
                return false;
            Symbols.Add(declaration.Identifier, declaration);
            return true;
        }

        public bool TryLookup(string ident, out Declaration declaration)
        {
            if (Symbols.TryGetValue(ident, out declaration))
                return true;
            if (Parent is null)
                return false;
            return Parent.TryLookup(ident, out declaration);
        }

        public ImmutableArray<Declaration> GetDeclarations()
            => Symbols.Values.ToImmutableArray();
    }
}
