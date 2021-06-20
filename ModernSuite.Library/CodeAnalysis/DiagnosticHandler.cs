using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSuite.Library.CodeAnalysis
{
    public static class DiagnosticHandler
    {
        /// <summary>
        /// All "info" messages.
        /// </summary>
        private static readonly List<string> _infos = new List<string>();
        /// <summary>
        /// All "warning" messages.
        /// </summary>
        private static readonly List<string> _warns = new List<string>();
        /// <summary>
        /// All errors.
        /// </summary>
        private static readonly List<string> _errors = new List<string>();

        /// <summary>
        /// Adds a new diagnostic.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="diagnosticKind">If it is an error, a warning, etc.</param>
        public static void Add(string message, DiagnosticKind diagnosticKind = DiagnosticKind.Info)
        {
            switch (diagnosticKind)
            {
            case DiagnosticKind.Info:
                _infos.Add(message);
                break;
            case DiagnosticKind.Warn:
                _warns.Add(message);
                break;
            case DiagnosticKind.Error:
                _errors.Add(message);
                break;
            }
        }

        /// <summary>
        /// Clears all messages.
        /// </summary>
        public static void Clear()
        {
            _infos.Clear();
            _warns.Clear();
            _errors.Clear();
        }

        /// <summary>
        /// Displays all messages.
        /// </summary>
        /// <returns>If there is any error.</returns>
        public static bool Display()
        {
            foreach (var info in _infos)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("INFO: ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(info);
                Console.ResetColor();
            }

            foreach (var warn in _warns)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("WARNING: ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(warn);
                Console.ResetColor();
            }

            foreach (var error in _errors)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("ERROR: ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
                Console.ResetColor();
            }

            return _errors.Any();
        }
    }
}
