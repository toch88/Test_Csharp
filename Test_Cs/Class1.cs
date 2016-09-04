using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Globalization;


namespace Test_Cs
{
    public delegate List<object> parsePackageDelegate(string str);

    class Package{
        public double time;
        public double sinValue;
        public double cosValue;
    }

    class Class1
    {
        private Queue<Package> que = new Queue<Package>();
        private SerialPort sp=new SerialPort();        
        string[] porty = SerialPort.GetPortNames();       
      
        private static System.Timers.Timer _timer;
        //private Thread=new Thread(); 

        public Class1()
        {          
            initSerialPort();

            Console.WriteLine("Main() invoked on thread {0}.", Thread.CurrentThread.ManagedThreadId);
            
            Console.WriteLine("Doing more work in Main()!");
          
            Console.ReadLine();
        }      
  
        public void initSerialPort()
        {
            _timer = new System.Timers.Timer();
            _timer.Interval = 17;
            _timer.Elapsed += _timer_Elapsed;
            
            if (porty.Length > 0)
            {
                foreach (string s in porty)
                {
                    Console.WriteLine("Dostępny jest port {0}", s);
                }
                Console.WriteLine("Wybrany zostanie port {0}", porty[0]);          
                sp.PortName = porty[0];
                sp.BaudRate = 256000;
                //sp.DataReceived += sp_DataReceived;
                sp.DataReceived += sp_DataReceived_by_Task;
                sp.Open();                
                Console.WriteLine("Otwarty");
                // Print out the ID of the executing thread.  
                _timer.Enabled = true;
            }
            else
            {               
                Console.WriteLine("Nie ma portów");
            }
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (this)
            {
                if (que.Count >= 1000)
                {
                    Console.Clear();
                    Console.WriteLine("{0}", que.Dequeue().time);
                }              
            } 
        }

        private void sp_DataReceived_by_Task(object sender, SerialDataReceivedEventArgs e)
        {
            var str = sp.ReadExisting();
            Task t = Task.Factory.StartNew(() => splitDataToFrame(str));
            t.Wait();
            
        }

        private Package parseFrame(string str)
        {
            Package package=new Package();
            str = str.Replace("<", "");
            str = str.Replace(">", "");
            var formatTokens = str.Split(new char[] { ':' });
            for (int i = 0; i <formatTokens.Length; i++)
            {
                
                switch (i)
                {

                    case 0: 
                    {
                        package.sinValue = Double.Parse(formatTokens[i], CultureInfo.InvariantCulture);                        
                        break; 
                    }
                    case 1:
                    {
                        package.sinValue = Double.Parse(formatTokens[i], CultureInfo.InvariantCulture);
                        break;
                    }
                    case 2:
                    {
                        package.cosValue = Double.Parse(formatTokens[i], CultureInfo.InvariantCulture);
                        break;
                    }
                    case 3:
                    {
                        package.time = Double.Parse(formatTokens[i], CultureInfo.InvariantCulture);
                        break;
                    }  

                    default:
                        break;
                }           
            }

            return package;

        }
       

        private void parsePackage(string str)
        {
            
            //var str = "<0.0712:0.233:0.871>";
            str = str.Replace("<", "");
            str = str.Replace(">", "");
            str = str.Replace("\n", "");
            var formatTokens = str.Split(new char[] { ':' });
           
            List<object> result = new List<object>();
            Console.WriteLine("--------------------------------");
            Console.WriteLine("{0}", Thread.CurrentThread.Name);
            for (int i=0; i < formatTokens.Length; i++)
            {
                Console.WriteLine("{0}", formatTokens[i]);
                result.Add(formatTokens[i]);
            }
            Console.WriteLine("--------------------------------");           
        }



        private void splitDataToFrame(string str)
        {

            var formatTokens = str.Split(new char[] { '\n' });
            //List<object> result = new List<object>();  
            lock (this)
            {
            //Console.WriteLine("------------Początek------------");                    
                //Console.WriteLine("{0}", Thread.CurrentThread.ManagedThreadId);
                for (int i = 0; i < formatTokens.Length; i++)
                {
                    //Console.WriteLine("{0}", formatTokens[i]);
                    if (formatTokens[i].Length >=25)
                    {
                       Task<Package> t=Task.Factory.StartNew<Package>(()=>(parseFrame(formatTokens[i])));
                       if (que.Count < 1000)
                       {
                           que.Enqueue(t.Result);
                       }
                       else
                       {
                           que.Dequeue();
                           que.Enqueue(t.Result);
                       }
                        
                    }
                }
               
                //Console.WriteLine("---------Koniec-----------------"); 
            }
            
        }


    }
}
