using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpCaml.DataAccess.Interface;
using Microsoft.SharePoint;

namespace SpCaml.DataAccess.Native
{
    public class SpNativeListAdapter : ISpListAdapter
    {
        #region ISpListAdapter Members

        public bool FetchInBatches
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int BatchCount
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public SpCaml.DataAccess.Caml.ListDef GetSchema()
        {
            throw new NotImplementedException();
        }

        public SpCaml.DataAccess.Caml.ChangeBatch GetChanges(string lastChangeToken, string batchId)
        {
            throw new NotImplementedException();
        }

        public SpCaml.DataAccess.Caml.ListItemCollection FillBatch(string batchId)
        {
            throw new NotImplementedException();
        }

        public void Update(SpCaml.DataAccess.Caml.UpdateBatch updateBatch)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
