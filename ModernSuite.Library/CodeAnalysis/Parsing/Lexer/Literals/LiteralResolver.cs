using System;
using System.Globalization;

namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Literals
{
    public sealed class LiteralResolver : Resolver<Literal>
    {
        public override Literal Parse(string text)
        {
            if (text.StartsWith("0x") || text.StartsWith("0X"))
            {
                text = text.Remove(0, 2);
                if (long.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var i64hResult))
                    return new IntLiteral { Value = i64hResult };
            }

            if (text.StartsWith("0b") || text.StartsWith("0B"))
            {
                text = text.Remove(0, 2);
                var val = 0L;
                var k = 0L;
                var valid = true;
                for (int i = 0; i < text.Length; i++)
                {
                    val = text[i] == '0' ? 0 : text[i] == '1' ? 1 : 0xEEBAD;
                    if (val == 0xEEBAD)
                    {
                        valid = false;
                        break;
                    }

                    if (val == 1)
                        k += (long)Math.Pow(2, text.Length - 1 - i);
                }

                if (valid)
                    return new IntLiteral { Value = k };
            }

            if (long.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var i64Result))
                return new IntLiteral { Value = i64Result };
            else if (text == "true")
                return new IntLiteral { Value = 1 };
            else if (text == "false")
                return new IntLiteral { Value = 0 };
            else
                return null;
        }
    }
}
