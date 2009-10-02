using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SpServerSync.Data
{
    public class SpSyncAdapterCollection : Collection<SpSyncAdapter>
    {
        public bool Contains(string listName)
        {
            foreach (SpSyncAdapter adapter in this)
                if (adapter.ListName == listName)
                    return true;

            return false;
        }

        public void Remove(string listName)
        {
            if (this.Contains(listName))
                base.Remove(this[listName]);
        }

        public SpSyncAdapter this[string listName]
        {
            get {

                if (listName == null)
                    throw new ArgumentNullException("listName");

                foreach (SpSyncAdapter adapter in this)
                    if (adapter.ListName == listName)
                        return adapter;

                throw new ArgumentException("Invalid list name");
            }
        }
    }
}
