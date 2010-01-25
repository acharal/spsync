
namespace Sp.Sync.Data
{
    public class ColumnMapping
    {
        public ColumnMapping()
        { 
        
        }

        public ColumnMapping(string serverColumn, string clientColumn)
        {
            ClientColumn = clientColumn;
            ServerColumn = serverColumn;
        }

        public string ClientColumn { set; get; }

        public string ServerColumn { set; get; }
    }
}
