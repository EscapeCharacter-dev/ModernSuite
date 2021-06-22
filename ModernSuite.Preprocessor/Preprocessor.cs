using System;
using System.IO;

namespace ModernSuite.Preprocessor
{
    public sealed class Preprocessor
    {
        public string Preprocess(string path)
        {
            var text = File.ReadAllText(path);
            var output = "";
            var position = 0;
            while (position < text.Length)
            {
                if (position == '#')
                {
                    position++;
                    var keyword = "";
                    while (!char.IsWhiteSpace(text[position]))
                        keyword += text[position++];
                    switch (keyword)
                    {
                    case "string":

                        break;
                    }
                }

                position++;
            }
        }
    }
}
