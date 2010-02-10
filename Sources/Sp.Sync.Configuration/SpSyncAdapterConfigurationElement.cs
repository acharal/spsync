using System;
using System.Configuration;

namespace Sp.Sync.Configuration
{
    public class SpSyncAdapterConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("list", IsRequired = true)]
        public string List
        {
            set { base["list"] = value; }
            get { return (string)base["list"]; }
        }

        [ConfigurationProperty("view", IsRequired = false)]
        public string View
        {
            set { base["view"] = value; }
            get { return (string)base["view"]; }
        }

        [ConfigurationProperty("table", IsRequired = true)]
        public string Table
        {
            set { base["table"] = value; }
            get { return (string)base["table"]; }
        }

        [ConfigurationProperty("rowguid", IsRequired = false)]
        public string RowGuidColumnName
        {
            set { base["rowguid"] = value; }
            get { return (string)base["rowguid"]; }
        }

        [ConfigurationProperty("columnmappings", IsKey=true)]
        public ColumnMappingConfigurationElementCollection ColumnMappings
        {
            set { base["columnmappings"] = value;  }
            get { return (ColumnMappingConfigurationElementCollection)base["columnmappings"];  }
        }
    }
}
