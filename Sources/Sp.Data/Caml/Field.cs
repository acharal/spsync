using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sp.Data.Caml
{
    public class Field : FieldRef
    {
        public string FieldType { set; get; }
        public string List { set; get; }
        public bool IsPrimaryKey { set; get; }
        public bool IsCalculated { set; get; }
        public bool IsIndexed { set; get; }
    }
}
