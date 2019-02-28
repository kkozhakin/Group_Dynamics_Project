using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace logic
{
    class WebParser
    {
        private volatile Manager manager;
        public WebParser()
        {
            manager = Manager.GetObj();
        }
        public bool alive = true;

        public void run()
        {
            while (true)
            {

                
               
                manager.inProcess.TryDequeue(out Link link);
                if (link == null)
                {
                    alive = false;
                    if (manager.end)
                    {
                        return;
                    }
                    Thread.Sleep(1000);
                    continue;
                }
                alive = true;

                if (link.ToString().EndsWith("//"))
                {
                    Interlocked.Increment(ref manager.scipped);
                    continue;
                }
                if (link.ToString() == (link.Parent?.ToString() +"/"))
                {
                    Interlocked.Increment(ref manager.scipped);
                    continue;
                }
                if (link.ToString() == link.Parent?.ToString())
                {
                    Interlocked.Increment(ref manager.scipped);
                    continue;
                }
                if (manager.links.ContainsKey(link))
                {
                    Interlocked.Increment(ref manager.scipped);

                    if (manager.links[link] != null)
                    {
                        manager.links[link].Add(link.Parent);
                    }
                    continue;
                }

                System.Console.WriteLine(link);

                Interlocked.Increment(ref manager.done);

                if (!manager.Fits(link))
                {
                    parseExternalLink(ref link);
                    
                }
                else
                {
                    parseInternalLink(ref link);
                }
            }
        }
        private void parseInternalLink(ref Link link)
        {
            string html;
            HttpWebResponse response;
            StreamReader respStream;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link.ToString());

                request.AllowAutoRedirect = true;
                response = (HttpWebResponse)request.GetResponse();
                respStream = new StreamReader(response.GetResponseStream());
                html = respStream.ReadToEnd();
                response.Close();
                respStream.Close();
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref manager.bad);
                link.ErrorComments = ex.Message;
                manager.anouncer(link);
                if (manager.links.ContainsKey(link))
                {
                    manager.links[link].Add(link);
                }
                else
                {
                    manager.links.TryAdd(link, new ConcurrentBag<Link>(){link.Parent});
                }
                

                return;
            }
            if (!manager.links.ContainsKey(link))
            {
                manager.links.TryAdd(link, null);
            }

            parseHTML(ref html, link);
        }


        private void parseExternalLink(ref Link link)
        {

            bool bad = false;
            
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(link.ToString());
                webRequest.AllowAutoRedirect = true;
                HttpWebResponse response1 = null;
                response1 = (HttpWebResponse)webRequest.GetResponse();
                if (response1.StatusCode.ToString() != "OK")
                {
                    link.ErrorComments = response1.StatusCode.ToString();
                    bad = true;
                }
                response1.Close();
            }
            catch (Exception ex)
            {
                link.ErrorComments = ex.Message;
                bad = true;
                Interlocked.Increment(ref manager.bad);

            }

            if (bad)
            {
                manager.anouncer(link);
                if (manager.links.ContainsKey(link))
                {
                    if (manager.links.ContainsKey(link))
                    {
                        manager.links[link].Add(link);
                    }
                    else
                    {
                        manager.links.TryAdd(link, new ConcurrentBag<Link>() { link.Parent });
                    }
                }
                else
                {
                    manager.links.TryAdd(link, new ConcurrentBag<Link>() { link.Parent });
                }
                
            }
            else
            {
                manager.links.TryAdd(link, null);
            }
        }

        private void parseHTML(ref string html, Link father)
        {
            MatchCollection m1 = Regex.Matches(html, @"(<a.*?>.*?</a>)", RegexOptions.Singleline);
            foreach (Match m in m1)
            {
                string value = m.Groups[1].Value;
                Match m2 = Regex.Match(value, @"href=\""(.*?)\""",
                    RegexOptions.Singleline);
                if (m2.Success)
                {
                    string sLink = m2.Groups[1].Value;
                    if (Skip(sLink))
                        continue;
                    Link link;
                    try
                    {
                        link = new Link(sLink, father);
                    }
                    catch(Exception e)
                    {
                        continue;
                    }


                    if(manager.links.ContainsKey(link))
                    {
                        continue;
                    }

                    manager.inProcess.Enqueue(link);
                }
            }
        }

        private bool Skip(string link)
        {
            string[] ends = {".pptx", ".ppt", ".pdf", ".doc", ".pdf", ".docx", ".xls"};
            string[] starts = { "mailto", "javascript", "obj:", "tel:", "#"};
            string[] contains = { "t-do.ru" , "goto="};

            foreach (var s in ends)
            {
                if (link.EndsWith(s))
                    return true;
            }

            foreach (var s in starts)
            {
                if (link.StartsWith(s))
                    return true;
            }

            foreach (var s in contains)
            {
                if (link.Contains(s))
                    return true;
            }

            return false;
        }
    }
}
