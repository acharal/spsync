using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SpServerSync.Data
{
    public class SyncColumnMappingCollection : Collection<ColumnMapping>
    {

        public int IndexOfServerColumn(string serverColumn)
        {
            throw new NotImplementedException();
        }

        public int IndexOfClientColumn(string clientColumn)
        {
            throw new NotImplementedException();
        }

        public ColumnMapping this[int index]
        {
            get {
                return base.Items[index];
            }
        }
    }

}
