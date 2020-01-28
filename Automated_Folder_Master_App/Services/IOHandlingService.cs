using System;
using System.IO;
using System.Linq;
using static System.Console;

namespace Master_Console.Services
{
    public static class IOHandlingService
    {
        public static void Introduction()
        {
            ForegroundColor = ConsoleColor.Green;
            WriteLine("Welcome to Deletor, I am your Deletor (I know I know). We will be deleting your disgusting files manually. Please follow the instructions below.");
            WriteLine("Remember, all your files will be deleted, even application related ones. On success, you should hear a beep. Press any key to continue.");
            
            ForegroundColor = ConsoleColor.Yellow;
            WriteLine("To exit, type 'quit'");
            
            ReadKey();
            Clear();

            ResetColor();
            Beep();
        }
        public static string PathInput()
        {
            WriteLine("Please provide the full path to the folder with your garbage files. You can just copy-paste it from WinExplorer.");
            var input = ReadLine();

            if (input == "quit")
            {
                Environment.Exit(0);
            }

            try
            {
                Path.GetFullPath(input);
                Beep();
                return input;
            }
            catch (Exception e)
            {
                ErrorHandlingService.ExceptionHandler(e);
            }
            return PathInput();
        }

        public static bool BinOrNotInput()
        {
            WriteLine("Shall the items be put into the bin instead of final deletion? Please type 'yes' or 'no'.");
            var input = ReadLine();
            if (input.Equals("yes"))
            {
                Beep();
                return true;
            }
            else if (input.Equals("no"))
            {
                Beep();
                return false;
            }

            return BinOrNotInput();
        }

        public static TimeSpan LifeSpanInput()
        {
            WriteLine("Please provide the number of days that should have passed since creation of these files.");
            var input = ReadLine();
            try
            {
                if (input == "quit")
                {
                    Environment.Exit(0);
                }

                if (input.All(char.IsDigit))
                {
                    Beep();
                    return TimeSpan.FromDays(double.Parse(input));
                } 
                else
                {
                    throw new FormatException();
                }
            }
            catch(FormatException e)
            {
                ErrorHandlingService.ExceptionHandler(e);
                return LifeSpanInput();
            }
        }

        public static void SuccessConfirmer()
        {
            Beep();
            Beep();
            Beep();
            Beep();
        }
    }
}
