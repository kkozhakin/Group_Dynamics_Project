using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logic
{
    class Restriction
    {
        public readonly string dom;
        public readonly string subDom;
        public readonly string way;

        public Restriction(string dom, string subDom, string way)
        {
            this.dom = dom;
            this.subDom = subDom;
            this.way = way;
        }
    }
}
