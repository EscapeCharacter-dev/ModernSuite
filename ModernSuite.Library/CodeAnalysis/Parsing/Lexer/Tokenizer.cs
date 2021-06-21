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

        private Lexable ParseChar()
        {
            Next();
            if (Current == '\0')
            {
                DiagnosticHandler.Add($"String is unfinished", DiagnosticKind.Error);
                return null;
            }
            var start = Position;
            var stringBuilder = new StringBuilder();
            var done = false;

            while (!done)
            {
                switch (Current)
                {
                case '\0':
                case '\r':
                case '\n':
                    DiagnosticHandler.Add($"String is unfinished", DiagnosticKind.Error);
                    return null;
                case '\'':
                    Next();
                    done = true;
                    break;
                case '\\':
                    Next();
                    if (Position >= Text.Length)
                    {
                        DiagnosticHandler.Add("Escape sequence in string is unfinished", DiagnosticKind.Error);
                        return null;
                    }
                    switch (Current)
                    {
                    case '"':
                        stringBuilder.Append('"');
                        break;
                    case '\'':
                        stringBuilder.Append('\'');
                        break;
                    case 'n':
                        stringBuilder.Append('\n');
                        break;
                    case 'r':
                        stringBuilder.Append('\r');
                        break;
                    case 't':
                        stringBuilder.Append('\t');
                        break;
                    case 'v':
                        stringBuilder.Append('\v');
                        break;
                    case '\\':
                        stringBuilder.Append('\\');
                        break;
                    case 'f':
                        stringBuilder.Append('\f');
                        break;
                    case 'b':
                        stringBuilder.Append('\b');
                        break;
                    case 'a':
                        stringBuilder.Append('\a');
                        break;
                    case '0':
                        stringBuilder.Append('\0');
                        break;
                    default:
                        DiagnosticHandler.Add($"Unrecognized escape sequence", DiagnosticKind.Error);
                        return null;
                    }
                    break;
                default:
                    stringBuilder.Append(Current);
                    Next();
                    break;
                }
            }

            var encoding = CharacterEncoding.ASCII;
            if (Current == 'a' || Current == 'u')
            {
                encoding = Current == 'u' ? CharacterEncoding.UTF8 : CharacterEncoding.ASCII;
                Next();
            }
            var str = stringBuilder.ToString();
            var encodingCSharp = encoding == CharacterEncoding.UTF8 ? Encoding.UTF8 : Encoding.ASCII;

            if (str.Length != 1)
            {
                DiagnosticHandler.Add($"Character literals can only be one character long", DiagnosticKind.Error);
                return null;
            }

            var b = encodingCSharp.GetBytes(str)[0];

            var token = new IntLiteral { Value = b };
            token.Line = Text[0..start].Split('\n').Count();
            token.Collumn = start / token.Line + 1;
            return token;
        }

        private Lexable ParseString()
        {
            Next();
            if (Current == '\0')
            {
                DiagnosticHandler.Add($"String is unfinished", DiagnosticKind.Error);
                return null;
            }
            var start = Position;
            var stringBuilder = new StringBuilder();
            var done = false;

            while (!done)
            {
                switch (Current)
                {
                case '\0':
                case '\r':
                case '\n':
                    DiagnosticHandler.Add($"String is unfinished", DiagnosticKind.Error);
                    return null;
                case '"':
                    Next();
                    done = true;
                    break;
                case '\\':
                    Next();
                    if (Position >= Text.Length)
                    {
                        DiagnosticHandler.Add("Escape sequence in string is unfinished", DiagnosticKind.Error);
                        return null;
                    }
                    switch (Current)
                    {
                    case '"':
                        stringBuilder.Append('"');
                        break;
                    case '\'':
                        stringBuilder.Append('\'');
                        break;
                    case 'n':
                        stringBuilder.Append('\n');
                        break;
                    case 'r':
                        stringBuilder.Append('\r');
                        break;
                    case 't':
                        stringBuilder.Append('\t');
                        break;
                    case 'v':
                        stringBuilder.Append('\v');
                        break;
                    case '\\':
                        stringBuilder.Append('\\');
                        break;
                    case 'f':
                        stringBuilder.Append('\f');
                        break;
                    case 'b':
                        stringBuilder.Append('\b');
                        break;
                    case 'a':
                        stringBuilder.Append('\a');
                        break;
                    case '0':
                        stringBuilder.Append('\0');
                        break;
                    default:
                        DiagnosticHandler.Add($"Unrecognized escape sequence", DiagnosticKind.Error);
                        return null;
                    }
                    break;
                default:
                    stringBuilder.Append(Current);
                    Next();
                    break;
                }
            }

            var token = new StringLiteral(stringBuilder.ToString());
            token.Line = Text[0..start].Split('\n').Count();
            token.Collumn = start / token.Line + 1;
            return token;
        }

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

            if (Current == '"')
                return ParseString();

            if (Current == '\'')
                return ParseChar();

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
                    DiagnosticHandler.Add($"({_token.Line},{_token.Collumn}) Unexpected characters '{_badLexable.Representation}'", DiagnosticKind.Error);
                return _token;
            }

            var has_digit = false;

            while (IsCurrentAnIdentifer || has_digit && Current == '.')
            {
                if (char.IsDigit(Current))
                    has_digit = true;
                Next();
            }
            var substr = Text[start..Position].Trim();
            var token = new LiteralResolver().Parse(substr) ?? new KeywordResolver().Parse(substr) as Lexable ?? new Identifier(substr);
            token.Line = Text[0..start].Split('\n').Count();
            token.Collumn = start / token.Line + 1;
            if (token is BadLexable badLexable)
                DiagnosticHandler.Add($"({token.Line},{token.Collumn}) Unexpected characters '{badLexable.Representation}'", DiagnosticKind.Error);
            return token;
        }
    }
}
