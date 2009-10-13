using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Synchronization.Data;
using System.Collections.ObjectModel;
using SpWSTest.Properties;
using Microsoft.Synchronization;

namespace SpWSTest
{
    class Program
    {
        static void Main(string[] args)
        {

            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener());
            SpSync.Data.Server.SpServerSyncProvider prov = new SpSync.Data.Server.SpServerSyncProvider();
            prov.ServiceCredentials = new System.Net.NetworkCredential("Administrator", "33z19@23", "VIANEXSRV");
            prov.BatchSize = 500;
            SyncServerInfo info = prov.GetServerInfo(null);
            Microsoft.Synchronization.Data.SqlServerCe.SqlCeClientSyncProvider client = new Microsoft.Synchronization.Data.SqlServerCe.SqlCeClientSyncProvider(Settings.Default.testConnectionString, true);
            Microsoft.Synchronization.SyncAgent agent = new Microsoft.Synchronization.SyncAgent(client, prov);
            //agent.Configuration.SyncTables.Add("Doctors", SyncDirection.Bidirectional);
            agent.Configuration.SyncTables.Add("Contacts", SyncDirection.Bidirectional);
            agent.SessionProgress += new EventHandler<Microsoft.Synchronization.SessionProgressEventArgs>(agent_SessionProgress);
            agent.StateChanged += new EventHandler<Microsoft.Synchronization.SessionStateChangedEventArgs>(agent_StateChanged);
            SyncStatistics stats = agent.Synchronize();
            PrintStats(stats);
            Console.Read();
        }

        private static void PrintStats(SyncStatistics stats)
        {
            Console.WriteLine("Sync started at " + stats.SyncStartTime + " and completed at " + stats.SyncCompleteTime);
            Console.WriteLine("Download Changes Applied: " + stats.DownloadChangesApplied);
            Console.WriteLine("Download Changes Failed : " + stats.DownloadChangesFailed);
            Console.WriteLine("Upload Changes Applied  : " + stats.UploadChangesApplied);
            Console.WriteLine("Upload Changes Failed   : " + stats.UploadChangesFailed);
            Console.WriteLine("Total Changes Downloaded: " + stats.TotalChangesDownloaded);
            Console.WriteLine("Total Changes Uploaded  : " + stats.TotalChangesUploaded);
        }

        static void agent_StateChanged(object sender, Microsoft.Synchronization.SessionStateChangedEventArgs e)
        {
            Console.WriteLine(e.SessionState.ToString());            
        }

        static void agent_SessionProgress(object sender, Microsoft.Synchronization.SessionProgressEventArgs e)
        {
            Console.WriteLine(e.SyncStage.ToString() + " " + e.PercentCompleted + "%");  
        }
    }
}

