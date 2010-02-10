using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Sp.Sync.Configuration
{
    public class SpSyncAdapterConfigurationElementCollection : ConfigurationElementCollection
    {
        protected override string ElementName
        {
            get
            {
                return "syncAdapter";
            }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SpSyncAdapterConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SpSyncAdapterConfigurationElement)element).Table;
        }
    }
}
