using ModernSuite.Library.CodeAnalysis.Parsing.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis
{
    public class Test
    {
        public void ParseTest()
        {
            var tokenizer = new Tokenizer(Console.ReadLine());
            Lexable lexable;
            do
            {
                lexable = tokenizer.NextToken();
                if (lexable.GetType() == typeof(EndOfFile))
                    break;
                Console.WriteLine($"{lexable.GetType()}: {lexable.Representation}");
            } while (lexable is not EndOfFile);
            foreach (var diagnostic in tokenizer.Diagnostics)
                Console.WriteLine(diagnostic);
        }
    }
}
