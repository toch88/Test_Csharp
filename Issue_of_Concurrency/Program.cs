using System;
using System.Threading;
using System.Windows;
using System.Windows.Forms;


namespace Issue_of_Concurrency
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("***** The Amazing Thread App *****\n");
            Console.Write("Do you want [1] or [2] threads? ");
            string threadCount = Console.ReadLine();

            // Name the current thread.
            Thread primaryThread = Thread.CurrentThread;
            primaryThread.Name = "Primary";

            // Display Thread info.
            Console.WriteLine("-> {0} is executing Main()", Thread.CurrentThread.Name);

            // Make worker class.
            PrinterClass p = new PrinterClass();
            switch(threadCount)
            {
                case "2":

            // Now make the thread.
                Thread backgroundThread = new Thread(new ThreadStart(p.PrintNumbers));
                backgroundThread.Name = "Secondary";
                backgroundThread.Start();
                break;
                case "1":
                p.PrintNumbers();
                break;
                default:
                Console.WriteLine("I don't know what you want...you get 1 thread.");
                goto case "1";
            }

            // Do some additional work.
            MessageBox.Show("I'm busy!", "Work on main thread...");
            Console.ReadLine();


        }
    }
}
