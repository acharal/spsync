﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data.SqlServerCe;
using Sp.Sync.Data;
using Sp.Sync.Data.Server;

namespace SpSyncSample
{
    public class SpSyncAgent : SyncAgent
    {

        private SyncTypeMappingCollection typeConnection = new SyncTypeMappingCollection();

        public SpSyncAgent( )
        {
            string sqlceConnString = "Data Source=local.sdf";

            LocalProvider  = new SqlCeClientSyncProvider(sqlceConnString);
            RemoteProvider = new SpServerSyncProvider("http://testintranet/crm3");
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
            SpSyncAdapter adapter1 = new SpSyncAdapter("Contacts");
            adapter1.TableName = "Contacts";

            foreach (TypeMapping m in typeConnection)
                adapter1.TypeMappings.Add(m);

            adapter1.TypeMappings.DefaultMapping = new TypeMapping("*", typeof(String), "nvarchar", 100);

            adapter1.DataColumns.Add("ID");
            adapter1.DataColumns.Add("GUID");
            adapter1.DataColumns.Add("ContentType");
            adapter1.DataColumns.Add("Title");
            adapter1.DataColumns.Add("owshiddenversion");
            adapter1.DataColumns.Add("FullName");


            // adapter1.RowGuidColumn = "GUID";
            // adapter1.FilterClause = "<Query><Where><Eq><FieldRef Name='ContentType' /> <Value Type='Text'>Person</Value></Eq></Where></Query>";

            (RemoteProvider as SpServerSyncProvider).SyncAdapters.Add(adapter1);
        }

        protected virtual void OnInitialized() 
        {
        
        }

    }
}

