using System;
using System.Collections.Generic;
using System.Threading;

namespace lab3
{
    class Program
    {
        static bool finish = false;
        static bool bEmpty = true;
        static String buffer;
        static List<String> MyMessage = new List<string>();
        static void Main(string[] args)
        {
            Thread[] writers = new Thread[3];
            Thread[] readers = new Thread[2];
            MyMessage.Add("AAAA");
            MyMessage.Add("BBBB");
            MyMessage.Add("CCCC");
            DateTime dt1, dt2;
            dt1 = DateTime.Now;
            for (int i = 0; i < writers.Length; i++)
            {
                writers[i] = new Thread(Writer);
                writers[i].Start(new object[] { i });
            }
           
            for (int i = 0; i < readers.Length; i++)
            {
                readers[i] = new Thread(Reader);
                readers[i].Start(new object[] { i });
            }

            for (int i = 0; i < writers.Length; i++)
            {
                writers[i].Join();
            }
            finish = true;
            for (int i = 0; i < readers.Length; i++)
            {
                readers[i].Join();
            }
            dt2 = DateTime.Now;
            TimeSpan ts = dt2 - dt1;
            Console.WriteLine("Time: {0}", ts.TotalMilliseconds);
        }

        static void Writer(object state)
        {
            int numWorker = (int)((object[])state)[0];
            int i = 0;
            int n = 100;
            while (i < n)
            {
                lock ("write")
                {
                    if (bEmpty)
                    {
                        buffer = "Writer:" + (numWorker + 1) + "Message:" + MyMessage[i++] + i;
                        bEmpty = false;
                    }
                }
            }
        }
        static void Reader(object state)
        {
            int numWorker = (int)((object[])state)[0];
            while (!finish)
            {
                if (!bEmpty)
                {
                    lock ("read")
                    {
                        if (!bEmpty)
                        {
                            bEmpty = true;
                            MyMessage.Add(buffer);
                            Console.WriteLine("Reader:" + (numWorker + 1) + buffer);
                        }
                    }
                }
            }
        }
    }
}

