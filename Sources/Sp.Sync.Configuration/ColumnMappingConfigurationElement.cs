using System;
using System.Configuration;

namespace Sp.Sync.Configuration
{

    public class ColumnMappingConfigurationElement : ConfigurationElement
    {

        [ConfigurationProperty("clientColumn")]
        public string ClientColumnName
        {
            get { return (string)base["clientColumn"]; }
            set { base["clientColumn"] = value; }
        }

        [ConfigurationProperty("serverColumn")]
        public string ServerColumnName
        {
            get { return (string)base["serverColumn"]; }
            set { base["serverColumn"] = value; }
        }
    }
}
