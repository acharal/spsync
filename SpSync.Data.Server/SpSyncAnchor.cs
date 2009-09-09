using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace SpSync.Data.Server
{
    [Serializable]
    public class SpSyncAnchor
    {
        public string PagingToken;
        public string NextChangesToken;
        public int PageNumber;
        public SpSyncAnchor NextChangesAnchor;

        public SpSyncAnchor(string changeToken)
        {
            NextChangesToken = changeToken;
            PagingToken = null;
            PageNumber = 1;
        }

        public SpSyncAnchor(string changeToken, string pageToken)
        {
            NextChangesToken = changeToken;
            PagingToken = pageToken;
            PageNumber = 1;
        }

        public SpSyncAnchor(string changeToken, string pageToken, int num)
        {
            NextChangesToken = changeToken;
            PagingToken = pageToken;
            PageNumber = num;
        }

        public static byte[] GetBytes(SpSyncAnchor anchor)
        {
            BinaryFormatter f = new BinaryFormatter();
            MemoryStream s = new MemoryStream();
            f.Serialize(s, anchor);
            return s.GetBuffer();
        }

        public static SpSyncAnchor GetAnchor(byte[] buffer)
        {
            if (buffer != null)
            {
                BinaryFormatter f = new BinaryFormatter();
                MemoryStream s = new MemoryStream(buffer);
                return (SpSyncAnchor)f.Deserialize(s);
            }
            else
            {
                return new SpSyncAnchor(null);
            }
        }
    }
}
