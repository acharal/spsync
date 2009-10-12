using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpServerSync.Data
{
    public class SpSyncAdapterCollection : Collection<SpSyncAdapter>
    {
        public bool Contains(string tableName)
        {
            foreach (SpSyncAdapter adapter in this)
                if (adapter.TableName == tableName)
                    return true;

            return false;
        }

        public void Remove(string tableName)
        {
            if (this.Contains(tableName))
                base.Remove(this[tableName]);
        }

        public int IndexOf(string tableName)
        {
            int i = 0;
            foreach (SpSyncAdapter adapter in this)
                if (adapter.TableName == tableName)
                    return i;
                else
                    i++;
            return -1;
        }

        public SpSyncAdapter this[string tableName]
        {
            get {

                if (tableName == null)
                    throw new ArgumentNullException("tableName");

                foreach (SpSyncAdapter adapter in this)
                    if (adapter.TableName == tableName)
                        return adapter;

                throw new ArgumentException("Invalid list name");
            }
        }
    }
}
