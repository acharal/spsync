using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Synchronization.Data.Server;
using Microsoft.Synchronization.Data;

namespace SpServerSync.Data
{
    /// <summary>
    /// Creates an SpSyncAdapter that are required to synchronize a client with a list in a Sharepoint server.
    /// </summary>
    public class SpSyncAdapterBuilder
    {
        /// <summary>
        /// Gets or sets the name of the base list for which to create a SyncAdapter object.
        /// </summary>
        public string ListName { set; get; }

        /// <summary>
        /// Gets a collection of data columns to be synchronized. 
        /// This enables you to synchronize a subset of the columns in the list.
        /// </summary>
        public SyncDataColumnCollection DataColumns { set; get; }

        /// <summary>
        /// Specifies the direction of synchronization from the perspective of the client, with a default direction of Bidirectional.
        /// </summary>
        public SyncDirection SyncDirection { set; get; }

        /// <summary>
        /// Gets or sets a column of uniqueidentifier data type that is used to identify rows.
        /// </summary>
        public string RowGuidColumn { set; get; }

        /// <summary>
        /// Creates a synchronization adapter for the table specified in ListName.
        /// </summary>
        /// <returns></returns>
        public SpSyncAdapter ToSpSyncAdapter()
        {
            throw new NotImplementedException();
        }
    }
}
