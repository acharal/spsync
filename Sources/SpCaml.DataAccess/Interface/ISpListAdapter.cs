using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpCaml.DataAccess.Caml;

namespace SpCaml.DataAccess.Interface
{
    public interface ISpListAdapter : IDisposable 
    {
        /// <summary>
        /// Choice to fetch the data in batches
        /// </summary>
        bool FetchInBatches { set; get; }

        /// <summary>
        /// Number in items to fetch per batch
        /// </summary>
        int BatchCount { set; get; }

        /// <summary>
        /// The name of the list
        /// </summary>
        string ListName { get; }

        /// <summary>
        /// The name of the site
        /// </summary>
        string SiteName { get; }

        /// <summary>
        /// The definition of the list
        /// </summary>
        /// <returns></returns>
        ListDef Schema { get; }

        /// <summary>
        /// Returns the changes since a token
        /// </summary>
        /// <returns></returns>
        ChangeBatch GetChanges(string lastChangeToken, string batchId);

        ListItemCollection FillBatch(string batchId);

        void Update(UpdateBatch updateBatch);
    }
}
