using System;
using System.Collections.ObjectModel;

namespace Sp.Sync.Data
{
    public class SpSyncTableAnchorCollection : Collection<SpSyncTableAnchor>
    {

        public SpSyncTableAnchorCollection()
        {

        }

        public bool Contains(string site, string tableName)
        {
            foreach (SpSyncTableAnchor tableAnchor in this)
                if (tableAnchor.TableName == tableName && tableAnchor.SiteName == site)
                    return true;
            return false;
        }

        public SpSyncAnchor this[string site, string tableName]
        { 
            get {
                foreach (SpSyncTableAnchor tableAnchor in this)
                    if (tableAnchor.TableName == tableName && tableAnchor.SiteName == site)
                        return tableAnchor.Anchor;

                return null;
            }

            set
            {
                foreach (SpSyncTableAnchor tableAnchor in this)
                {
                    if (tableAnchor.TableName == tableName && tableAnchor.SiteName == site)
                    {
                        tableAnchor.Anchor = value;
                        return;
                    }
                }

                SpSyncTableAnchor newAnchor = new SpSyncTableAnchor();
                newAnchor.TableName = tableName;
                newAnchor.SiteName = site;
                newAnchor.Anchor = value;
                this.Add(newAnchor);
            }
        }
    }
}
