using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpCaml.DataAccess.Caml
{
    public class ListItemCollection : List<ListItem>
    {
        public ListItemCollection(IEnumerable<ListItem> collection) 
            : base(collection) { }
        
        public int ItemCount;
        public string NextPage;
    }
}
