using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Console;
using System.Runtime;
using System.Diagnostics;

namespace Try1
{
    class Link
    {
        public readonly Link init;
        private string prot = "http://";
        private string pre;
        public string main;
        private string way;
        

        public Link(string link, Link init)
        {
            this.init = init;
            if (link.Contains("http://"))
            {
                link = link.Replace("http://", "");
                ParseNormal(link);
            }
            else if (link.Contains("https://"))
            {
                link = link.Replace("https://", "");
                prot = "https://";
                ParseNormal(link);
            }
            else if (link.StartsWith("//"))
            {

                link = link.Substring(2, link.Length - 2);
                ParseNormal(link);

            }
            else if (link.StartsWith("/"))
            {
                link = link.Substring(1, link.Length - 1);
                ParseHard(link);
            }
            else
            {
                ParseHard(link);
            }
//             else
//             {
//                 throw new  FormatException($"bad link: {init} - {link}");
//             }
        }

        private void ParseNormal(string link)
        {
            int dot_1 = link.IndexOf('.');
            pre = link.Substring(0, dot_1);
            link = link.Substring(dot_1+1, link.Length-dot_1-1);
            int slash = link.IndexOf('/');
            if (slash == -1)
            {
                main = link;
            }
            else
            {
                main = link.Substring(0, slash);
                if (slash != link.Length - 1)
                {
                    way = link.Substring(slash + 1, link.Length - slash - 1);
                }
            }
        }

        private void ParseHard(string link)
        {
            prot = init.prot;
            pre = init.pre;
            main = init.main;
            way = init.way;
            if (way!=null)
            {
                way += "/";
            }
            way += link;
        }


        public override string ToString()
        {
            return prot + pre + "." + main +"/" + way;
        }


    }


    static class LinkFinder
    {
        public static void Find(ref HashSet<Link> watched, ref Queue<Link> queue, ref Stopwatch sw)
        {
            if (queue.Count == 0)
            {
                return;
            }
            Link link = queue.Dequeue();
            if (watched.Contains(link))
            {
                return;
            }
            watched.Add(link);
           // WriteLine(link);
            WebClient client = new WebClient();
            
            byte[] buffer;
            try
            {
                sw.Start();
                buffer = client.DownloadData(link.ToString());
                sw.Stop();
            }
            catch (WebException e)
            {
                WriteLine(e.Message + $"   Bad link {link.init} - {link}");
                return;
            }
            catch 
            {
                WriteLine($"   Bad link {link.init} - {link}");
                return;
            }
            if (link.main != "hse.ru")
            {
                return;
            }
            string html = System.Text.Encoding.Default.GetString(buffer);
            

            MatchCollection m1 = Regex.Matches(html, @"(<a.*?>.*?</a>)",
                RegexOptions.Singleline);

            foreach (Match m in m1)
            {
                string value = m.Groups[1].Value;
                Match m2 = Regex.Match(value, @"href=\""(.*?)\""",
                    RegexOptions.Singleline);
                if (m2.Success)
                {
                    if(m2.Groups[1].Value.StartsWith("mailto"))
                    {
                        continue;
                    }
                    if (m2.Groups[1].Value.StartsWith("javascript"))
                    {
                        continue;
                    }
                    if (m2.Groups[1].Value.StartsWith("obj:"))
                    {
                        continue;
                    }
                    if (m2.Groups[1].Value.StartsWith("tel:"))
                    {
                        continue;
                    }
                    if (m2.Groups[1].Value.Contains("t-do.ru"))
                    {
                        continue;
                    }

                    if (m2.Groups[1].Value == "")
                    {
                        continue;
                    }

                    try
                    {
                        Link lk = new Link(m2.Groups[1].Value, link);

                        LinkAdder(lk, ref watched, ref queue);
                    }
                    catch(FormatException e)
                    {
                        WriteLine(e.Message);
                    }
                }
            }
        }
        public static void LinkAdder(Link link, ref HashSet<Link> watched, ref Queue<Link> queue)
        {
            if (watched.Contains(link))
            {
                return;
            }
            queue.Enqueue(link);
        }
    }
}