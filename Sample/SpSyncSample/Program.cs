using System;

namespace SpSyncSample
{
    class Program
    {
        static void Main(string[] args)
        {
            SpSyncAgent agent = new SpSyncAgent();
            agent.Configuration.SyncTables.Add("Contacts", Microsoft.Synchronization.Data.SyncDirection.DownloadOnly);
            agent.SessionProgress += new EventHandler<Microsoft.Synchronization.SessionProgressEventArgs>(agent_SessionProgress);
            agent.Synchronize();
        }

        static void agent_SessionProgress(object sender, Microsoft.Synchronization.SessionProgressEventArgs e)
        {
            Console.WriteLine(e.PercentCompleted);
        }
    }
}
