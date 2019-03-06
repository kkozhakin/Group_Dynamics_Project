using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using PageScrapper;

namespace logic
{
    class Manager
    {
        public delegate void changes();
        public changes skippedChanges;
        public delegate void searchIsEnd();
        public searchIsEnd endOfSearch;
        public delegate void GetBroken(Link link);
        public GetBroken anouncer;
        readonly static Manager manager;
        private Manager() { }
        static Manager()
        {
            manager = new Manager();
        }

        public static Manager GetObj()
        {
            return manager;
        }
        public void setRestriction(string dom, string subdom, string way)
        {
            rs = new Restriction(dom, subdom, way);
        }

        public bool Fits(Link link)
        {
            if (!link.MainDomen.Contains(rs.dom))
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

        public void BeginProcess(Object _n)
        {
            end = false;
            int n = (int)_n;
            System.Console.WriteLine("Process started");
            Thread[] tasks = new Thread[n];
            WebParser[] parsers = new WebParser[n];

            for (int i = 0; i < n; i++)
            {
                WebParser parser = new WebParser();
                parsers[i] = parser;
                tasks[i] = new Thread(parser.run);
                tasks[i].Start();
            }




            while (true)
            {
                Thread.Sleep(2500);
                skippedChanges();
                bool active = false;
                foreach (var parser in parsers)
                {
                    active = active || parser.alive;
                }
                if (!active)
                { 
                    end = true;
                    skippedChanges();
                    endOfSearch();
                    return;
                }
                if (end)
                {
                    return;
                }
            }
        }

        public string GetBadLinks()
        {
            StringBuilder s = new StringBuilder();
            foreach(var pair in links)
            {
                string tmp = "";
                if (pair.Value != null) 
                {
                    if (pair.Key.ErrorComments.Contains("503"))
                    {
                        continue;
                    }
                    tmp += "Bad link: " + pair.Key.ToString() + Environment.NewLine;
                    tmp += $"\tComment: {pair.Key.ErrorComments}{Environment.NewLine}";
                    foreach (var lk in pair.Value.Distinct())
                    {
                        tmp += $"\tSource: {lk}{Environment.NewLine}";
                    }
                    s.Append(tmp);
                    s.Append($"_______________{Environment.NewLine}");
                }            
            }
            return s.ToString();
        }
        
        public void Clear()
        {
            inProcess = new ConcurrentQueue<Link>();
            links = new ConcurrentDictionary<Link, ConcurrentBag<Link>>();
            done = 0;
            bad = 0;
            scipped = 0;
            Set.URL = null;
            Set.subdom = "";
            Set.dom = "";
            Set.way = "";
            Set.thread_num = 4;
            skippedChanges();
        }

        public volatile bool end = false;
        public volatile int done = 0;
        public volatile int bad = 0;
        public volatile int scipped = 0;
        private Restriction rs;
        public volatile ConcurrentQueue<Link> inProcess = new ConcurrentQueue<Link>();
        public volatile ConcurrentDictionary<Link, ConcurrentBag<Link>> links = new ConcurrentDictionary<Link, ConcurrentBag<Link>>();        
    }

    
}

