using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Extensions;
using SpCaml.DataAccess.Caml;
using SpCaml.DataAccess.Interface;
using System.Xml.Linq;
using System.Web.Services.Protocols;

namespace SpCaml.DataAccess.Implementation.WS
{
    /// <summary>
    /// Used for the communication between sharepoint and application.
    /// </summary>
    public class SpListAdapter : ISpListAdapter
    {
        private Lists listWS;
        
        private string _listName = null;
        private string _spSiteName = null;

        private bool fetchInBatches = true;

        private int rowLimit = 100;

        private string ListasmxSuffix = "/_vti_bin/Lists.asmx";

        public System.Net.ICredentials Credentials = System.Net.CredentialCache.DefaultCredentials;

        protected virtual Lists ListsService
        {
            get { return listWS;  }
        }

        #region ISpListAdapter Members

        public SpListAdapter(string spSite, string listName)
        {
            _listName = listName;
            _spSiteName = spSite;

            listWS = new Lists();

            listWS.Url = this.Url;
            listWS.Credentials = Credentials;
        }
        
        public string ListName
        {
            get { return _listName; }
        }

        public string SiteName
        {
            get { return _spSiteName; }
        }

        protected virtual string Url
        {
            get { return SiteName + ListasmxSuffix; }
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

        public ListDef Schema
        {
            get
            {
                return ListsService.GetList(ListName).GetXElement().GetCamlListDef();
            }
        }

        public ChangeBatch GetChanges(string lastChangeToken, string batchId)
        {
            QueryOptions queryOptions = new QueryOptions();
            
            queryOptions.PagingToken = batchId;
            
            XElement changes = listWS.GetListItemChangesSinceToken(ListName, 
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

            XElement result = listWS.GetListItems(ListName, null, null, null, BatchCount.ToString(), queryOptions.GetCamlQueryOptions(), null).GetXElement();
            return result.GetCamlListItemCollection();
        }

        public void Update(UpdateBatch updateBatch)
        {
            listWS.UpdateListItems(ListName, updateBatch.GetCamlUpdateBatch());
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
