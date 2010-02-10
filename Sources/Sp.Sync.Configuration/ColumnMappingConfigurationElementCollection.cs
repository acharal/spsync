using System;
using System.Configuration;

namespace Sp.Sync.Configuration
{
    public class ColumnMappingConfigurationElementCollection : ConfigurationElementCollection
    {

        protected override bool ThrowOnDuplicate
        {
            get { return true; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ColumnMappingConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ColumnMappingConfigurationElement)element).ClientColumnName;
        }
    }
}
