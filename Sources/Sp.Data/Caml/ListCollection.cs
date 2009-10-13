using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sp.Data.Caml
{
    public class ListCollection : List<List>
    {
        public ListCollection(IEnumerable<List> collection)
            : base(collection) { }
    }
}
