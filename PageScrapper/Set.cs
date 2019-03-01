using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using logic;

namespace PageScrapper
{
    static class Set
    {
        public delegate void start_button();
        public static start_button start_button_enable;


        public static Link URL; //= new Link("");
        public static string subdom = "";
        public static string dom = "";
        public static string way = "";
        public static int thread_num = 4;

    }
}
