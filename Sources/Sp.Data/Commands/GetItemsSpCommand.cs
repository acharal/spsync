using System;
using System.Data;

namespace Sp.Data
{
    public class GetItemsSpCommand : SpCommand
    {
        public override IDataReader ExecuteReader()
        {
            return base.ExecuteReader();
        }

        public override IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return base.ExecuteReader(behavior);
        }
    }
}
