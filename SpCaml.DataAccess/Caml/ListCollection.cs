﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpCaml.DataAccess.Caml
{
    public class ListCollection : List<List>
    {
        public ListCollection(IEnumerable<List> collection)
            : base(collection) { }
    }
}
