namespace ModernSuite.Library.CodeAnalysis.Parsing.Lexer.Operators
{
    public sealed partial class OperatorResolver : Resolver<Operator>
    {
        public override Operator Parse(string text)
        {
            return text switch
            {
                "+" => new PlusOperator(),
                "-" => new MinusOperator(),
                "*" => new StarOperator(),
                "/" => new SlashOperator(),
                "%" => new PercentageOperator(),
                "&" => new AndOperator(),
                "|" => new PipeOperator(),
                "!" => new BangOperator(),
                "^" => new HelmetOperator(),
                "&&" => new AndsOperator(),
                "||" => new PipesOperator(),
                "<=" => new ArrowEqualLeftOperator(),
                ">=" => new ArrowEqualRightOperator(),
                "<" => new ArrowLeftOperator(),
                ">" => new ArrowRightOperator(),
                ">>" => new ArrowsRightOperator(),
                "<<" => new ArrowsLeftOperator(),
                "==" => new EqualsOperator(),
                "=" => new EqualOperator(),
                "!=" => new BangEqualOperator(),
                "++" => new PlusPlusOperator(),
                "--" => new MinusMinusOperator(),
                "(" => new ParenthesisOpenOperator(),
                ")" => new ParenthesisClosedOperator(),
                "[" => new SquareBracketOpenOperator(),
                "]" => new SquareBracketClosedOperator(),
                "{" => new BracketOpenOperator(),
                "}" => new BracketClosedOperator(),
                "->" => new ArrowOperator(),
                "=>" => new LargeArrowOperator(),
                "?" => new WhatOperator(),
                ":" => new ColonOperator(),
                // "::" => new CubeOperator(), not used in 2021.06
                ";" => new SemicolonOperator(),
                "~" => new TildaOperator(),
                "." => new DotOperator(),
                "," => new CommaOperator(),
                "@" => new AtOperator(),
                "$" => new DollarOperator(),
                _ => null
            };
        }
    }
}
