using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpCaml.DataAccess.Caml
{
    public class ListDef
    {
        public List List { set; get; }
        public IEnumerable<Field> Fields { set; get; }
    }
}
