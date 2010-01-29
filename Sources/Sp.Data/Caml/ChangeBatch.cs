using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sp.Data.Caml
{
    public class ChangeBatch
    {
        public int MinTimeBetweenSyncs;
        public int RecommendedTimeBetweenSyncs;
        public ChangeLog ChangeLog;
        public ListItemCollection ChangedItems;

        public bool HasSchemaChanges()
        {
            return ChangeLog != null && ChangeLog.NewListDef != null;
        }

        public bool HasMoreChanges()
        {
            return (ChangeLog != null && ChangeLog.MoreChanges) || HasMoreData();
        }

        public bool HasMoreData()
        {
            return ChangedItems.NextPage != null;
        }

        public string NextPage
        {
            get { return ChangedItems.NextPage; }
        }
        
        public string NextChangeBatch
        {
            get { return ChangeLog.NextLastChangeToken; }
        }

        public string CurrentChangeBatch;

        public const string FirstChangeBatch = null;
        public const string FirstPage = null;
    }

}
