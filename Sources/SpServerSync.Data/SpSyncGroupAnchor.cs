using System;
using System.IO;
using System.Xml.Serialization;

namespace Sp.Sync.Data
{
    public class SpSyncGroupAnchor : SpSyncTableAnchorCollection
    {
        public int BatchCount;

        public static SpSyncGroupAnchor Deserialize(byte[] buffer)
        {
            using (MemoryStream m = new MemoryStream(buffer))
            {
                var ser = new XmlSerializer(typeof(SpSyncGroupAnchor));
                object o = ser.Deserialize(m);
                return o as SpSyncGroupAnchor;
            }
        }

        public static byte[] Serialize(SpSyncGroupAnchor x)
        {
            using (MemoryStream m = new MemoryStream())
            {
                var ser = new XmlSerializer(typeof(SpSyncTableAnchorCollection));
                ser.Serialize(m, x);
                return m.GetBuffer();
            }
        }
    }
}
