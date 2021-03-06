﻿using System;
using System.IO;
using System.Linq;
using static System.Console;

namespace Master_Console.Services
{
    public class IOHandlingService
    {
        public const string BinOrNot = "Shall the items be put into the bin instead of final deletion? Please type 'yes' or 'no'.";
        public const string FolderDelOrNot = "Please advise, shall we delete the folder itself too, or just the contents? Please type 'yes or 'no'.";
        private const string _quitString = "quit";
        public void Introduction()
        {
            ForegroundColor = ConsoleColor.Green;
            WriteLine("Welcome to Deletor, I am your Deletor (I know I know). We will be deleting your disgusting files manually. Please follow the instructions below.");
            WriteLine("Remember, all your files will be deleted, even application related ones. On success, you should hear a beep. Press any key to continue.");

            ForegroundColor = ConsoleColor.Red;
            WriteLine("To exit, type 'quit'");
            
            ResetColor();
            ReadKey();
            Clear();

            Beep();
        }
        public string PathInput()
        {
            ColoredQuestion("Please provide the full path to the folder with your garbage files. You can just copy-paste it from WinExplorer.");
            var input = ReadLine();

            if (input.Equals(_quitString))
            {
                Quit();
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

        public bool YesOrNoInput(string question)
        {
            ColoredQuestion(question);
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
            else if (input.Equals(_quitString))
            {
                Quit();
            }

            return YesOrNoInput(question);
        }

        public TimeSpan LifeSpanInput()
        {
            ColoredQuestion("Please provide the number of days that should have passed since creation of these files.");
            var input = ReadLine();
            try
            {
                if (input.Equals(_quitString))
                {
                    Quit();
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

        public void SuccessConfirmer()
        {
            Beep();
            Beep();
            Beep();
            Beep();
        }

        private void ColoredQuestion(string text)
        {
            ForegroundColor = ConsoleColor.DarkCyan;
            WriteLine(text);
            ResetColor();
        }

        private void Quit()
        {
            Environment.Exit(0);
        }
    }
}
