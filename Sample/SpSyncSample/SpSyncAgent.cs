using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data.SqlServerCe;
using SpServerSync.Data;

namespace SpSyncSample
{
    public class SpSyncAgent : SyncAgent
    {

        private SyncTypeMappingCollection typeConnection = new SyncTypeMappingCollection();

        public SpSyncAgent( )
        {
            string sqlceConnString = "Data Source=local.sdf";

            LocalProvider  = new SqlCeClientSyncProvider(sqlceConnString);
            RemoteProvider = new SpServerSyncProvider("http://testintranet", "/");
            (RemoteProvider as SpServerSyncProvider).BatchSize = 100;
            InitializeTypeMappings();
            InitializeAdapters();
            OnInitialized();
        }

        protected virtual void InitializeTypeMappings()
        {
            typeConnection.Add(new TypeMapping("Counter", typeof(int), "int"));
            typeConnection.Add(new TypeMapping("Number",  typeof(int), "int"));
            typeConnection.Add(new TypeMapping("Integer", typeof(int), "int"));
            typeConnection.Add(new TypeMapping("Lookup",  typeof(int), "int"));
            typeConnection.Add(new TypeMapping("User",    typeof(int), "int"));
            typeConnection.Add(new TypeMapping("Note",    typeof(String), "ntext"));
            typeConnection.Add(new TypeMapping("Guid",    typeof(Guid), "uniqueidentifier"));
            typeConnection.Add(new TypeMapping("DateTime", typeof(DateTime), "datetime"));
            typeConnection.Add(new TypeMapping("Recurrence", typeof(bool), "bit"));
            typeConnection.Add(new TypeMapping("AllDayEvent", typeof(bool), "bit"));
            typeConnection.Add(new TypeMapping("Boolean", typeof(bool), "bit"));
            typeConnection.Add(new TypeMapping("Text", typeof(String), "nvarchar", 100));
            typeConnection.Add(new TypeMapping("URL", typeof(String), "nvarchar", 100));
        }

        protected virtual void InitializeAdapters()
        {
            SpSyncAdapter adapter1 = new SpSyncAdapter("User Information List");
            adapter1.TableName = "User Information List";
            adapter1.ViewName = "c2456ed9-f4a8-44cd-bc41-c889d99fd706";

            foreach (TypeMapping m in typeConnection)
                adapter1.TypeMappings.Add(m);
            
            adapter1.TypeMappings.DefaultMapping = new TypeMapping("*", typeof(String), "nvarchar", 100);
            
            // adapter1.RowGuidColumn = "GUID";
            adapter1.FilterClause = "<Query><Where><Eq><FieldRef Name='ContentType' /> <Value Type='Text'>Person</Value></Eq></Where></Query>";

            (RemoteProvider as SpServerSync.Data.SpServerSyncProvider).SyncAdapters.Add(adapter1);
        }

        protected virtual void OnInitialized() 
        {
        
        }

    }
}
