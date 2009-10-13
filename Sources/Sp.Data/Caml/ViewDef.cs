using System;
using System.Collections.Generic;
using System.Xml;

namespace Sp.Data.Caml
{
    public class ViewDef
    {
        public string Name { set; get; }
        public string DisplayName { set; get; }
        public ListDef ListDef { set; get; }
        public List<FieldRef> ViewFields { set; get; }
        public XmlNode Query { set; get; }
    }
}
