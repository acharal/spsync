using System;
using System.Collections.ObjectModel;

namespace Sp.Sync.Data
{
    public class SpSyncTableAnchorCollection : Collection<SpSyncTableAnchor>
    {

        public SpSyncTableAnchorCollection()
        {

        }

        public bool Contains(string tableName)
        {
            foreach (SpSyncTableAnchor tableAnchor in this)
                if (tableAnchor.TableName == tableName)
                    return true;
            return false;
        }

        public SpSyncAnchor this[string tableName]
        { 
            get {
                foreach (SpSyncTableAnchor tableAnchor in this)
                    if (tableAnchor.TableName == tableName)
                        return tableAnchor.Anchor;

                return null;
            }

            set
            {
                foreach (SpSyncTableAnchor tableAnchor in this)
                {
                    if (tableAnchor.TableName == tableName)
                    {
                        tableAnchor.Anchor = value;
                        return;
                    }
                }

                SpSyncTableAnchor newAnchor = new SpSyncTableAnchor();
                newAnchor.TableName = tableName;
                newAnchor.Anchor = value;
                this.Add(newAnchor);
            }
        }
    }
}
