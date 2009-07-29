﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpCaml.DataAccess.Caml
{
    public class UpdateBatch : List<UpdateItem>
    {
        public UpdateBatch(IEnumerable<UpdateItem> items)
            : base(items) { }

        public bool ContinueOnError = true;
    }
}
