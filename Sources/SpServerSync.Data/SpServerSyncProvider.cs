using System;
using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Synchronization.Data;

namespace SpServerSync.Data
{
    /// <summary>
    /// Abstracts a sharepoint server synchronization provider that
    /// communicates with a SharePoint server and shields the client from
    /// the specific implementation of the server
    /// </summary>
    /// <remarks>
    /// The principal activities of the server synchronization are:
    /// * get the changes from the sharepoint server using the build-in change tracking of sharepoint
    /// * applies the incremental changes to the server
    /// * transforms the lists of sharepoint to relational tables
    /// </remarks>
    public class SpServerSyncProvider : ServerSyncProvider, IDisposable
    {
        public EventHandler<SelectingChangesEventArgs> SelectingChanges;
        
        public EventHandler<ChangesSelectedEventArgs> ChangesSelected;
        
        public EventHandler<ApplyingChangesEventArgs> ApplyingChanges;
        
        public EventHandler<ApplyChangeFailedEventArgs> ApplyChangeFailed;
        
        public EventHandler<ChangesAppliedEventArgs> ChangesApplied;
        
        /// <summary>
        /// Occurs during the selection and application of changes for a synchronization group at the server.
        /// </summary>
        public EventHandler<SyncProgressEventArgs> SyncProgress;

        /// <summary>
        /// Gets or sets the batch size (in rows) that is used by commands that retrieve changes from the server database. 
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Gets or sets a SyncSchema object that contains information about the table schema on the server. 
        /// </summary>
        public SyncSchema Schema { get; set; }

        /// <summary>
        /// Initializes a new instance of the SpServerSyncProvider class.
        /// </summary>
        public SpServerSyncProvider()
        { 
        
        }

        /// <summary>
        /// Applies inserts, updates, and deletes for a synchronization group to the server database. 
        /// </summary>
        /// <param name="groupMetadata">A SyncGroupMetadata object that contains metadata about the synchronization group.</param>
        /// <param name="dataSet">A DataSet object that contains the changes to be applied to the server database for each table in the synchronization group.</param>
        /// <param name="syncSession">A SyncSession object that contains synchronization session variables, such as the ID of the client that is synchronizing.</param>
        /// <returns>A SyncContext object that contains synchronization data and metadata.</returns>
        public override SyncContext ApplyChanges(SyncGroupMetadata groupMetadata, DataSet dataSet, SyncSession syncSession)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Releases all resources used by the SpServerSyncProvider. 
        /// </summary>
        public override void Dispose()
        {
            
        }
        
        /// <summary>
        /// Selects for a table in the server database the inserts, updates, and deletes to apply to the client database for a synchronization group. 
        /// </summary>
        /// <param name="groupMetadata">A SyncGroupMetadata object that contains metadata about the synchronization group.</param>
        /// <param name="syncSession">A SyncSession object that contains synchronization session variables, such as the ID of the client that is synchronizing.</param>
        /// <returns>A SyncContext object that contains synchronization data and metadata.</returns>
        public override SyncContext GetChanges(SyncGroupMetadata groupMetadata, SyncSession syncSession)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a SyncSchema object that contains the schema for each table specified. 
        /// </summary>
        /// <param name="tableNames">A collection of table names for which the server provider should get the schema.</param>
        /// <param name="syncSession">A SyncSession object that contains synchronization session variables, such as the ID of the client that is synchronizing.</param>
        /// <returns>A SyncSchema object that contains the schema for each table that is specified.</returns>
        public override SyncSchema GetSchema(Collection<string> tableNames, SyncSession syncSession)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Raises the SyncProgress event. 
        /// </summary>
        /// <param name="args">A SyncProgressEventArgs object that contains event data.</param>
        protected virtual void OnSyncProgress( SyncProgressEventArgs args )
        {
            if (SyncProgress != null)
                SyncProgress(this, args);
        }
    }
}
