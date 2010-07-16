using System;
using Microsoft.Synchronization.Data.SqlServerCe;

namespace SyncState
{
    public class SyncState
    {
        SqlCeClientSyncProvider localProvider;

        public SyncState(SqlCeClientSyncProvider _localProvider)
        {
            localProvider = _localProvider;

            TableStatus activityTable = new TableStatus(localProvider, "Activity", "serverid=0", "InTrash=1");
        }

        public class TableStatus
        {
            SqlCeClientSyncProvider localProvider;
            int newcount = 0;
            int modifiedcount = 0;
            int deletedcount = 0;
            string TableName = "";
            string NewCondition = "";
            string DeletedCondition = "";
            SqlCeClientSyncProvider localProvider;

            public TableStatus(SqlCeClientSyncProvider _localProvider, string _TableName , string _newCondition, string _DeletedCondition)
            {
                localProvider = _localProvider;
                TableName = _TableName;
                NewCondition = _newCondition;
                DeletedCondition = _DeletedCondition;

                RefreshStatus();
            }

            public TableStatus(int _new, int _modified, int _deleted)
            {
                NewCount = _new;
                ModifiedCount = _modified;
                DeletedCount = _deleted;
            }
            public void RefreshStatus()
            {

                if (localProvider != null)
                {
                    try
                    {
                        Microsoft.Synchronization.Data.SyncContext c = localProvider.GetChanges(TableName);
                        ModifiedCount = c.DataSet.Tables[TableName].Rows.Count;
                        NewCount = c.DataSet.Tables[TableName].Select(NewCondition).Length;
                        DeletedCount = c.DataSet.Tables[TableName].Select(DeletedCondition).Length;
                    }
                    catch { }
                }
                if (ModifiedCount != 0)
                {
                    ModifiedCount -= NewCount;
                    ModifiedCount -= DeletedCount;
                }
            }

            public int NewCount { get { return newcount; } set { newcount = value; } }
            public int ModifiedCount { get { return modifiedcount; } set { modifiedcount = value; } }
            public int DeletedCount { get { return deletedcount; } set { deletedcount = value; } }

        }
    }
}