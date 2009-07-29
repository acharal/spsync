using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Extensions;
using SpCaml.DataAccess.Caml;
using SpCaml.DataAccess.Interface;
using System.Xml.Linq;

namespace SpCaml.DataAccess.Implementation.WS
{
    /// <summary>
    /// Used for the communication between sharepoint and application.
    /// </summary>
    public class SpListAdapter : ISpListAdapter
    {
        private Lists listWS;
        
        private string listName = null;

        private bool fetchInBatches = true;

        private int rowLimit = 100;

        private string ListasmxSuffix = "/_vti_bin/Lists.asmx";

        public System.Net.ICredentials Credentials = System.Net.CredentialCache.DefaultCredentials;

        #region ISpListAdapter Members

        public SpListAdapter(string spsite, string list)
        {
            listName = list;
            listWS = new Lists();
            listWS.Url = spsite + ListasmxSuffix;
            listWS.Credentials = Credentials;
        }

        public bool FetchInBatches
        {
            get
            {
                return fetchInBatches;
            }
            set
            {
                fetchInBatches = value;
            }
        }

        public int BatchCount
        {
            get
            {
                return rowLimit;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("BatchCount cannot be negative or zero", "BatchCount");

                rowLimit = value;
            }
        }

        public ListDef GetSchema()
        {
            return listWS.GetList(listName).GetXElement().GetCamlListDef();
        }

        public ChangeBatch GetChanges(string lastChangeToken, string batchId)
        {
            QueryOptions queryOptions = new QueryOptions();
            
            queryOptions.PagingToken = batchId;
            
            XElement changes = listWS.GetListItemChangesSinceToken(listName, 
                null, 
                null, 
                null,
                BatchCount.ToString(), 
                queryOptions.GetCamlQueryOptions(), 
                lastChangeToken, 
                null).GetXElement();

            return changes.GetCamlChangeBatch();
        }

        public ListItemCollection FillBatch(string batchId)
        {
            QueryOptions queryOptions = new QueryOptions();

            if (FetchInBatches)
                queryOptions.PagingToken = batchId;

            XElement result = listWS.GetListItems(listName, null, null, null, BatchCount.ToString(), queryOptions.GetCamlQueryOptions(), null).GetXElement();
            return result.GetCamlListItemCollection();
        }

        public void Update(UpdateBatch updateBatch)
        {
            listWS.UpdateListItems(listName, updateBatch.GetCamlUpdateBatch());
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            listWS.Dispose();
        }

        #endregion

    }
}
