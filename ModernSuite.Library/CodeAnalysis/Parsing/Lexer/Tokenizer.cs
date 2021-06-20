using ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Keywords;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Literals;
using ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer
{
    public sealed class Tokenizer
    {
        public string Text { get; }
        public int Position { get; private set; } = 0;
        public char Peek(int offset = 0)
            => Position + offset < Text.Length ? Text[Position + offset] : '\0';
        public char Current => Peek();
        public char Lookahead => Peek(1);
        private void Next() => Position++;
        public List<string> Diagnostics { get; } = new List<string>();
        public Tokenizer(string text)
        {
            Text = text;
        }

        private bool IsCurrentAnIdentifer => char.IsLetterOrDigit(Current) || Current == '_';

        public Lexable NextToken()
        {
            while (char.IsWhiteSpace(Current))
                Next();
            if (Current == '\0')
            {
                Next();
                return new EndOfFile();
            }

            var start = Position;

            if (!IsCurrentAnIdentifer && Current != '\0' && !char.IsWhiteSpace(Current))
            {
                char old = Current;
                if (Current == ',' || Current == '.' || Current == '(' || Current == ')' || Current == '[' ||
                    Current == ']' || Current == '{' || Current == '}' || Current == ';' || Current == ':' ||
                    Current == '@' || Current == '$' || Current == '~')
                    Next();
                else
                    while (Current == old && Current != '\0' ||
                        (old == '-' && Current == '>') || (old == '=' && Current == '>')
                        || ((old == '<' || old == '>' || old == '!') && Current == '='))
                        Next();
                var opSubstr = Text[start..Position].Trim();
                var _token = new OperatorResolver().Parse(opSubstr) as Lexable ?? new BadLexable(opSubstr);
                _token.Line = Text[0..start].Split('\n').Count();
                _token.Collumn = start / _token.Line + 1;
                if (_token is BadLexable _badLexable)
                    Diagnostics.Add($"({_token.Line},{_token.Collumn}) Unexpected characters '{_badLexable.Representation}'");
                return _token;
            }

            while (IsCurrentAnIdentifer)
                Next();
            var substr = Text[start..Position].Trim();
            var token = new LiteralResolver().Parse(substr) ?? new KeywordResolver().Parse(substr) as Lexable ?? new Identifier(substr);
            token.Line = Text[0..start].Split('\n').Count();
            token.Collumn = start / token.Line + 1;
            if (token is BadLexable badLexable)
                Diagnostics.Add($"({token.Line},{token.Collumn}) Unexpected characters '{badLexable.Representation}'");
            return token;
        }
    }
}
