using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpServerSync.Data
{
    public class ColumnMapping
    {
        public ColumnMapping(string serverColumn, string clientColumn)
        {
            ClientColumn = clientColumn;
            ServerColumn = serverColumn;
        }

        public string ClientColumn;

        public string ServerColumn;
    }
}
