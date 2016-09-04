using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Synchronization_Monitor_Type
{

    public class PrinterClass
    {
        private object threadLock = new object(); //blokowacz!

        public void PrintNumbers()
        {
         Monitor.Enter(threadLock);
         try
         {
            // Display Thread info.
             Console.WriteLine("-> {0} is executing PrintNumbers()",
            Thread.CurrentThread.Name);
            // Print out numbers.
            Console.Write("Your numbers: ");
         for (int i = 0; i < 10; i++)
         {
             Random r = new Random();
             Thread.Sleep(1000 * r.Next(5));
             Console.Write("{0}, ", i);
         }
            Console.WriteLine();
         }
         finally
         {
            Monitor.Exit(threadLock);
         }
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*****Synchronizing Threads *****\n");
            PrinterClass p = new PrinterClass();
            // Make 10 threads that are all pointing to the same
            // method on the same object.
            Thread[] threads = new Thread[5];
            for (int i = 0; i < 5; i++)
            {
                threads[i] = new Thread(new ThreadStart(p.PrintNumbers));
                threads[i].Name = string.Format("Worker thread #{0}", i);
            }
            // Now start each one.
            foreach (Thread t in threads)
                t.Start();
            Console.ReadLine();
        }
    }
}
