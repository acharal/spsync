using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpCaml.DataAccess.Caml
{
    public class Field
    {
        public string FieldType { set; get; }
        public string List { set; get; }
        public string Name { set; get; }
        public string DisplayName { set; get; }
        public bool IsRequired { set; get; }
        public bool IsReadOnly { set; get; }
        public bool IsHidden { set; get; }
        public bool IsPrimaryKey { set; get; }
        public bool IsCalculated { set; get; }
        public bool IsIndexed { set; get; }
    }
}
