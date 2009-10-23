using System;
using System.Collections.ObjectModel;

namespace Sp.Sync.Data
{
    public class SyncTypeMappingCollection : Collection<TypeMapping>
    {
        public TypeMapping DefaultMapping { set; get; }

        public int IndexOfFieldType(string fieldType)
        {
            int i = 0;

            foreach (TypeMapping mapping in this)
            {
                if (mapping.FieldType == fieldType)
                    return i;
                else
                    i++;
            }

            return -1;
        }

        public TypeMapping this[int index]
        {
            get {
                return base.Items[index];
            }
        }
    }
}
