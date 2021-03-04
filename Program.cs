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
            AutoResetEvent evFull = new AutoResetEvent(false);
            AutoResetEvent evEmpty = new AutoResetEvent(true);
            SemaphoreSlim semReader = new SemaphoreSlim(0,2);
            SemaphoreSlim semWriter = new SemaphoreSlim(0,3);
            MyMessage.Add("AAAA");
            MyMessage.Add("BBBB");
            MyMessage.Add("CCCC");
            for (int i = 0; i < writers.Length; i++)
            {
                writers[i] = new Thread(Writer);
                writers[i].Start(new object[] { i, semWriter });
            }
           
            for (int i = 0; i < readers.Length; i++)
            {
                readers[i] = new Thread(Reader);
                readers[i].Start(new object[] { i, semReader });
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
        }

        static void Writer(object state)
        {
            int numWorker = (int)((object[]) state)[0];
            var semWriter = ((object[])state)[1] as SemaphoreSlim;
            int i = 0;
            int n = 3;
            while (i < n)
            {
                if (bEmpty)
                {
                    semWriter.Wait(500);
                    if (bEmpty)
                    {
                        bEmpty = false;
                        buffer = "Writer:" + (numWorker + 1) + "Message:" + MyMessage[i++] + i;
                    }
                    semWriter.Release(1);
                }
            }
        }
        static void Reader(object state)
        {
            int numWorker = (int)((object[])state)[0];
            var semReader = ((object[])state)[1] as SemaphoreSlim;
            while (!finish)
            {
                if (!bEmpty)
                {
                    semReader.Wait(500);
                    if (!bEmpty)
                    {
                        bEmpty = true;
                        MyMessage.Add(buffer);
                        Console.WriteLine("Reader:" + (numWorker + 1) + buffer);
                    }
                    semReader.Release(1);
                }
            }
        }
    }
}

