using System;
using System.Configuration;

namespace Sp.Sync.Configuration
{
    public class SpSyncConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection=true, IsKey=false)]
        public SpSyncAdapterConfigurationElementCollection SyncAdapters
        {
            get { return (SpSyncAdapterConfigurationElementCollection)base[""]; }
            set { base[""] = value; }
        }
    }
}
