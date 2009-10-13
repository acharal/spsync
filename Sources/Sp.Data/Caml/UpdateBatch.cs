using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sp.Data.Caml
{
    public class UpdateBatch : List<UpdateItem>
    {
        public UpdateBatch() { }

        public bool ContinueOnError = true;

        private int nextID = 0;

        public UpdateItem CreateNewItem()
        {
            UpdateItem item = new UpdateItem();
            item.ID = nextID;
            nextID++;
            return item;
        }
    }
}
