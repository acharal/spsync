using System;
using System.Collections.ObjectModel;

namespace SpServerSync.Data
{
    public class SyncTypeMappingCollection : Collection<TypeMapping>
    {
        public int IndexOfFieldType(string fieldType)
        {
            throw new NotImplementedException();
        }

        public TypeMapping this[int index]
        {
            get {
                return base.Items[index];
            }
        }
    }
}
