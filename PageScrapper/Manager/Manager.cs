using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
namespace logic
{
    class Manager
    {
        readonly static Manager manager;
        private Manager() {}
        static  Manager()
        {
            manager = new Manager();
        }

        public static Manager GetObj()
        {
            return manager;
        }
        public void setRestriction(string dom, string subdom, string way)
        {
            rs = new Restriction(dom, subdom,  way);
        }
        
        public bool Fits(Link link)
        {
            if (rs.dom != link.MainDomen)
            {
                return false;
            }
            if (!link.SubDomen.EndsWith(rs.subDom))
            {
                return false;
            }
            if (!link.Way.StartsWith(rs.way))
            {
                return false;
            }
            return true;
        }

        public void SetURL(string url)
        {
            inProcess.Enqueue(new Link(url));
        }

        public void BeginProcess(int n)
        {


            Thread[] tasks = new Thread[n];
            WebParser[] parsers = new WebParser[n]; 

            for(int i=0; i<n; i++)
            {

                WebParser parser = new WebParser();
                parsers[i] = parser;
                tasks[i] = new Thread(parser.run);
                tasks[i].Start();
            }



            
            while (true)
            {
                Thread.Sleep(2500);
                bool active = false;
                foreach(var parser in parsers)
                {
                    active = active || parser.alive;
                }
                if (!active)
                {
                    end = true;
                    return;
                }
            }
            
            
        }


        public volatile bool end = false;
        public volatile int done = 0;
        private Restriction rs;
        public volatile ConcurrentQueue<Link> inProcess = new ConcurrentQueue<Link>();
        public volatile ConcurrentDictionary<Link, ConcurrentBag<Link>> links = new ConcurrentDictionary<Link, ConcurrentBag<Link>>();        
    }

    
}

