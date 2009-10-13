using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sp.Data.Caml
{
    public class FieldRef
    {
        public string Name { set; get; }
        public string DisplayName { set; get; }
        public bool IsRequired { set; get; }
        public bool IsReadOnly { set; get; }
        public bool IsHidden { set; get; }
    }
}
