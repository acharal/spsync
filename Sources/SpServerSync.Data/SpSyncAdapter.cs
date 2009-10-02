using System;
using System.Data;
using Sp.Data;

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
        public SyncColumnMappingCollection ColumnMappings { get; private set; }

        /// <summary>
        /// Gets or sets the name of the Sharepoint list
        /// </summary>
        public string ListName { set; get; }

        /// <summary>
        /// Gets or sets the description of the synchronization adapter
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        /// Gets or sets the current connection to the sharepoint server
        /// </summary>
        public SpConnection Connection { set; get; }

        /// <summary>
        /// Gets or sets the current transaction to the sharepoint server
        /// </summary>
        public SpTransaction Transaction { set; get; }

        /// <summary>
        /// Initializes a new SpSyncAdapter object
        /// </summary>
        public SpSyncAdapter()
        {
            ColumnMappings = new SyncColumnMappingCollection();
        }

        /// <summary>
        /// Initializes a new SpSyncAdapter object and adapts it to a list
        /// </summary>
        /// <param name="listName"></param>
        public SpSyncAdapter(string listName)
        {
            if (listName == null)
                throw new ArgumentNullException("listName");

            ListName = listName;
            ColumnMappings = new SyncColumnMappingCollection();
        }

        /// <summary>
        /// Gets the client column name that corresponds to the specified server column name. 
        /// </summary>
        /// <param name="serverColumn">the name of the column in server</param>
        /// <returns>the name of the column in client</returns>
        public string GetClientColumnFromServerColumn(string serverColumn)
        {
            int index = ColumnMappings.IndexOfServerColumn(serverColumn);
            
            if (index > -1)
                return ColumnMappings[index].ClientColumn;
            else
                return serverColumn;
        }

        /// <summary>
        /// Populates the schema information for the table that is specified in TableName.
        /// </summary>
        /// <param name="table">the table to populate</param>
        /// <param name="connection">the connection object to sharepoint</param>
        /// <returns>a datatable object that contains the schema information</returns>
        public DataTable FillSchema(DataTable table, SpConnection connection)
        {
            throw new NotImplementedException();
        }

        public DataTable SelectIncrementalInserts()
        {
            throw new NotImplementedException();
        }

        public DataTable SelectIncrementalUpdates()
        {
            throw new NotImplementedException();
        }

        public DataTable SelectIncrementalDeletes()
        {
            throw new NotImplementedException();
        }

        public void Update( ){ }

        public void Delete() { } 

    }
}
