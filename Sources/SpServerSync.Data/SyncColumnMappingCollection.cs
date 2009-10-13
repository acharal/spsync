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
            int index = 0;
            foreach (ColumnMapping mapping in this)
            {
                if (mapping.ServerColumn == serverColumn)
                    return index;
                else
                    index++;
            }

            return -1;
        }

        public int IndexOfClientColumn(string clientColumn)
        {
            int index = 0;
            foreach (ColumnMapping mapping in this)
            {
                if (mapping.ClientColumn == clientColumn)
                    return index;
                else
                    index++;
            }
            return -1;
        }


        public ColumnMapping this[int index]
        {
            get {
                return base.Items[index];
            }
        }
    }

}
