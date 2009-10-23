
namespace Sp.Sync.Data
{
    public class ColumnMapping
    {
        public ColumnMapping(string serverColumn, string clientColumn)
        {
            ClientColumn = clientColumn;
            ServerColumn = serverColumn;
        }

        public string ClientColumn { private set; get; }

        public string ServerColumn { private set; get; }
    }
}
