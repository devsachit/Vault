using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault
{
public    class Index
    {
        public int index { get; set; }
        public Node.Selection selection { get; set; }
        public Index(int index,Node.Selection selection)
        {
            this.index = index;
            this.selection = selection;
        }
    }
}
