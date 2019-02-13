using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Console;

namespace Try1
{
    class Program
    {
        static void Main(string[] args)
        {
            Link lk = new Link("https://strategy.hse.ru/", null);
            HashSet<Link> watched = new HashSet<Link>();
            Queue<Link> que = new Queue<Link>();
            que.Enqueue(lk);
            Stopwatch sw = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            for (int i=1; i<10000; i++)
            {
                LinkFinder.Find(ref watched, ref que, ref sw);
                if (i % 10 == 0)
                {
                    WriteLine($"Open = {(sw.ElapsedMilliseconds / 1000.0).ToString()}, Total = {(sw2.ElapsedMilliseconds / 1000.0).ToString()}");
                    sw.Reset();
                    sw2.Restart();
                }
                WriteLine($"{i}, {que.Peek()}");
                
            }



            //string html = System.Text.Encoding.Default.GetString(buffer);
           

            Console.ReadLine();
        }



    }

    




    
}

