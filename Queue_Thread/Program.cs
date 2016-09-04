using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Queue_Thread
{
    public class PrinterClass
    {
        private object threadLock = new object(); //blokowacz!
        public static int current=0;
        public string name;
        public void PrintNumbers()
        {
            Monitor.Enter(threadLock);
            try
            {
                // Display Thread info.
                Console.WriteLine("-> {0} is executing PrintNumbers()", Thread.CurrentThread.ManagedThreadId);
                // Print out numbers.
                Console.WriteLine("Your numbers: ");
                for (int i = 0; i < 10; i++)
                {
                    Random r = new Random();
                    Thread.Sleep(1000 * r.Next(5));
                    Console.Write("{0} :, ", i);
                    Console.WriteLine("Current :{0} and name is {1} : ", ++current, name);
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
            Console.WriteLine("***** Fun with the CLR Thread Pool *****\n");
            Console.WriteLine("Main thread started. ThreadID = {0}", Thread.CurrentThread.ManagedThreadId);
            PrinterClass[] p=new PrinterClass[10];
            WaitCallback workItem = new WaitCallback(PrintTheNumbers);
            // Queue the method ten times.
            for (int i = 0; i < 10; i++)
            {
              p[i] = new PrinterClass();
              p[i].name = string.Format("{0}", i);
              ThreadPool.QueueUserWorkItem(workItem, p[i]);
            }
            Console.WriteLine("All tasks queued");
            Console.ReadLine();
        }

        static void PrintTheNumbers(object state)
        {
            PrinterClass task = (PrinterClass)state;
            task.PrintNumbers();
            
        }
    }
}
