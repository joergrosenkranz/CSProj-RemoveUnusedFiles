using System;

namespace RemoveUnusedFiles
{
    public static class Log
    {
        public static void Error(string message, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            try
            {
                Console.Error.WriteLine(message, args);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        public static void Info(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }
    }
}
