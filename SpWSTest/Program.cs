using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Synchronization.Data;
using System.Collections.ObjectModel;
using SpWSTest.Properties;

namespace SpWSTest
{

    class Program
    {
        static void Main(string[] args)
        {
            SpSync.Data.Server.SpServerSyncProvider prov = new SpSync.Data.Server.SpServerSyncProvider();
            Microsoft.Synchronization.Data.SqlServerCe.SqlCeClientSyncProvider client = new Microsoft.Synchronization.Data.SqlServerCe.SqlCeClientSyncProvider(Settings.Default.testConnectionString, true);
            Microsoft.Synchronization.SyncAgent agent = new Microsoft.Synchronization.SyncAgent(client, prov);
            agent.Configuration.SyncTables.Add("Doctors", SyncDirection.Bidirectional);
            agent.Synchronize();
        }
    }
}
