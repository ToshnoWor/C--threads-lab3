using System;
using System.Collections.Generic;
using System.Threading;

namespace lab3
{
    class Program
    {
        static bool finish = false;
        static String buffer;
        static List<String> MyMessage = new List<string>();
        static void Main(string[] args)
        {
            Thread[] writers = new Thread[3];
            Thread[] readers = new Thread[2];
            AutoResetEvent evFull = new AutoResetEvent(false);
            AutoResetEvent evEmpty = new AutoResetEvent(true);
            MyMessage.Add("AAAA");
            MyMessage.Add("BBBB");
            MyMessage.Add("CCCC");
            for (int i = 0; i < writers.Length; i++)
            {
                writers[i] = new Thread(Writer);
                writers[i].Start(new object[] { i, evFull, evEmpty });
            }
           
            for (int i = 0; i < readers.Length; i++)
            {
                readers[i] = new Thread(Reader);
                readers[i].Start(new object[] { i, evFull, evEmpty });
            }

            for (int i = 0; i < writers.Length; i++)
            {
                writers[i].Join();
            }
            finish = true;
            evFull.Set();
            for (int i = 0; i < readers.Length; i++)
            {
                readers[i].Join();
            }
        }

        static void Writer(object state)
        {
            int numWorker = (int)((object[]) state)[0];
            var evFull = ((object[])state)[1] as AutoResetEvent;
            var evEmpty = ((object[])state)[2] as AutoResetEvent;
            int i = 0;
            int n = 3;
            while (i < n)
            {
                evEmpty.WaitOne();
                buffer = "Writer:" + (numWorker + 1) + "Message:" + MyMessage[i++] + i;
                evFull.Set();
            }
        }
        static void Reader(object state)
        {
            int numWorker = (int)((object[])state)[0];
            var evFull = ((object[])state)[1] as AutoResetEvent;
            var evEmpty = ((object[])state)[2] as AutoResetEvent;
            while (!finish)
            {
                if (finish) 
                    break;
                evFull.WaitOne();
                MyMessage.Add(buffer);
                Console.WriteLine("Reader:" + (numWorker + 1) + buffer);
                evEmpty.Set();
            }
        }
    }
}

