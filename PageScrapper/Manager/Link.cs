using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logic
{
    class Link
    {

        private Link parent;
        private string protocol = "http://";
        private string subDomen;
        private string mainDomen;
        private string way="";
        private string errorComments;

        public string SubDomen { get => subDomen;}
        public string MainDomen { get => mainDomen;}
        public string ErrorComments { get => errorComments; set => errorComments = value; }
        internal Link Parent { get => parent;}
        public string Way { get => way; }

        public string getErrorComments() => errorComments;


       

        public Link(string link, Link init)
        {
            parent = init;
            _parseStage1(link);
        }

        public Link(string link)
        {
            _parseStage1(link);
        }

        private void _parseStage1(string link)
        {
            if (link.EndsWith("/"))
            {
                link = link.Substring(0, link.Length - 1);
            }
            if (link.Contains("http://"))
            {
                link = link.Replace("http://", "");
                _parseAbsoluteURL(link);
            }
            else if (link.Contains("https://"))
            {
                link = link.Replace("https://", "");
                protocol = "https://";
                _parseAbsoluteURL(link);
            }
            else if (link.StartsWith("//"))
            {

                link = link.Substring(2, link.Length - 2);
                _parseAbsoluteURL(link);

            }
            else if (link.StartsWith("/"))
            {
                link = link.Substring(1, link.Length - 1);
                _parseRelativeURL(link, false);
            }
            else
            {
                _parseRelativeURL(link, true);
            }
        }

        private void _parseAbsoluteURL(string link)
        {
            int dot_1 = link.IndexOf('.');
            subDomen = link.Substring(0, dot_1);
            link = link.Substring(dot_1 + 1, link.Length - dot_1 - 1);
            int slash = link.IndexOf('/');
            if (slash == -1)
            {
                mainDomen = link;
            }
            else
            {
                mainDomen = link.Substring(0, slash);
                if (slash != link.Length - 1)
                {
                    way = link.Substring(slash + 1, link.Length - slash - 1);
                }
            }
        }

        private void _parseRelativeURL(string link, bool add)
        {
            protocol = parent.protocol;
            subDomen = parent.subDomen;
            mainDomen = parent.mainDomen;
            
            if (add)
            {
                way = parent.way;
                if (way != null)
                {
                    way += "/";
                }
            }
            way += link;
        }


        public override string ToString()
        {
            return (protocol + subDomen + "." + mainDomen + "/" + way).Replace(" ", "");
        }

        public override int GetHashCode()
        {
            return (subDomen + "." + mainDomen + "/" + way).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Link lk = (Link)obj;
            return (subDomen + "." + mainDomen + "/" + way).Equals(lk.subDomen + "." + lk.mainDomen + "/" + lk.way);
        }
    }
}
