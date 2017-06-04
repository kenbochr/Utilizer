using PirReg;
using System;
using System.Timers;
using System.Diagnostics;
using PirReg.Model;

namespace ConsoleApp
{
    class Program
    {
        static PirRegProgram pirRegProgram = new PirRegProgram();

        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "Udnyttelsesgrader";

            Write("Program til registrering af undnyttelsesgrader \t\t\t\t\t", true, ConsoleColor.Blue);
            Initiate();
        }

        private static void Initiate()
        {
            int seconds = 5;
            Write(string.Format("Press a key within {0} seconds, if you want to edit settings...", seconds), false);
            var timer = new Timer(1000);
            timer.Elapsed += delegate(Object sender, ElapsedEventArgs e)
            {
                seconds--;
                Write(string.Format("\rPress a key within {0} seconds, if you want to edit settings...", seconds), false);
                if (seconds == 0)
                {
                    timer.Stop();
                    StartProgram();
                }
            };
            timer.Start();
            Console.ReadKey();
            if (seconds > 0)
            {
                timer.Stop();
                EditSettings();
            }
        }

        private static void EditSettings()
        {
            Write("\n\nSelect one of the two keys:", true);
            Write("  A: Open xml file that contains settings", true);
            Write("  B: Open xml file the contains settings, in Eplorer", true);
            
            var key = Console.ReadKey().Key.ToString();
            if (key == "A")
                Process.Start(Data.SettingsXmlPath.FullName);
            else if (key == "B")
                Process.Start(Data.SettingsXmlPath.Directory.FullName);
        }

        private static void StartProgram()
        {
            Write("\n\nStart analysis:", true, ConsoleColor.DarkGreen);
            pirRegProgram.StartAnalysis();

            Console.WriteLine();
            var seconds = 20;
            var timer = new Timer(1000);
            timer.Elapsed += delegate(Object sender, ElapsedEventArgs e)
            {
                seconds--;
                Write(string.Format("\rShuts down in {0} sekunder. Press a key to shut down now.", seconds), false);
                if (seconds == 0)
                    Environment.Exit(0);
                
            };
            timer.Start();
            Console.ReadKey();
        }

        public static void Write(string text, bool newLine = true, ConsoleColor backgroundColor = ConsoleColor.Black, ConsoleColor foregroundColor = ConsoleColor.White)
        {
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;
            if (newLine)
                Console.WriteLine(text);
            else
                Console.Write(text);

            Console.ResetColor();
        }
    }
}