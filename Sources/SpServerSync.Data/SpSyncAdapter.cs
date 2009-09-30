using System;
using System.Data;
using Microsoft.Synchronization.Data.Server;

namespace SpServerSync.Data
{
    /// <summary>
    /// The SpSyncAdapter serves as the bridge between the synchronization provider
    /// and the connection to a sharepoint server.
    /// </summary>
    public class SpSyncAdapter
    {
        /// <summary>
        /// Gets a collection of SyncColumnMapping objects for the table. 
        /// These objects map columns in a server table to the corresponding columns in a client table.
        /// </summary>
        public SyncColumnMappingCollection ColumnMappings { get; }

        /// <summary>
        /// Gets or sets the name of the Sharepoint list
        /// </summary>
        public string ListName { set; get; }

        /// <summary>
        /// Gets or sets the description of the synchronization adapter
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        /// Gets the client column name that corresponds to the specified server column name. 
        /// </summary>
        /// <param name="serverColumn">the name of the column in server</param>
        /// <returns>the name of the column in client</returns>
        public string GetClientColumnFromServerColumn(string serverColumn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Populates the schema information for the table that is specified in TableName.
        /// </summary>
        /// <param name="table">the table to populate</param>
        /// <returns>a datatable object that contains the schema information</returns>
        public DataTable FillSchema(DataTable table)
        {
            throw new NotImplementedException();
        }


    }
}
