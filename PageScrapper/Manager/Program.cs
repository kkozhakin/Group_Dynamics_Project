using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
namespace logic
{
    class Program
    {
        
        static void Main(string[] args)
        {


            Manager manager = Manager.GetObj();
            //https://lang.hse.ru/englishman
            manager.setRestriction("hse.ru", "", "cn");
            manager.SetURL("https://www.hse.ru/cn");
            manager.BeginProcess(5);

            WriteLine("Done");
            ReadLine();
        }

        public static void BadLink(Link link)
        {
            Console.WriteLine($"Brocken link: {link}\n" +
                $"Source: {link.Parent}\n" +
                $"Comment: {link.ErrorComments}\n" +
                $"------------------------------------");
        }
        public static void BadString(string str, Link parent)
        {
            WriteLine($"Bad string: {str}\n" +
                $"Source: {parent}");
        }
       
    }
}
