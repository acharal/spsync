using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Synchronization.Data;

namespace SpWSTest.Mobile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            SpSync.Data.Server.SpServerSyncProvider prov = new SpSync.Data.Server.SpServerSyncProvider();
            prov.ServiceCredentials = new System.Net.NetworkCredential("nangelc1it", "p@n@th@s13", "VIANEX");
            prov.BatchSize = 100;
            SyncServerInfo info = prov.GetServerInfo(null);
            Microsoft.Synchronization.Data.SqlServerCe.SqlCeClientSyncProvider client = new Microsoft.Synchronization.Data.SqlServerCe.SqlCeClientSyncProvider("Data Source=test.sdf", true);
            Microsoft.Synchronization.SyncAgent agent = new Microsoft.Synchronization.SyncAgent(client, prov);
            //agent.Configuration.SyncTables.Add("Doctors", SyncDirection.Bidirectional);
            agent.Configuration.SyncTables.Add("Accounts", SyncDirection.DownloadOnly);
            agent.Configuration.SyncTables.Add("Contacts", SyncDirection.DownloadOnly);
            agent.SessionProgress += new EventHandler<Microsoft.Synchronization.SessionProgressEventArgs>(agent_SessionProgress);
            agent.StateChanged += new EventHandler<Microsoft.Synchronization.SessionStateChangedEventArgs>(agent_StateChanged);
            SyncStatistics stats = agent.Synchronize();
        }


        void agent_StateChanged(object sender, Microsoft.Synchronization.SessionStateChangedEventArgs e)
        {
            label1.Text = e.SessionState.ToString();
        }

        int i = 1;

         void agent_SessionProgress(object sender, Microsoft.Synchronization.SessionProgressEventArgs e)
        {
            label1.Text = e.PercentCompleted + "% completed";
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = e.PercentCompleted;
            Console.WriteLine(i + ". " + e.PercentCompleted + "% completed");
            i++;
        }
    }
}