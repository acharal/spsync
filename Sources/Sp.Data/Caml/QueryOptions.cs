﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sp.Data.Caml
{
    public class QueryOptions
    {
        public bool DateInUtc = false;
        public string PagingToken = null;
        public bool IncludeMandatoryColumns = true;
    }
}
