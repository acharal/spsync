using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SpSync.Data.Server
{
    [XmlRoot("anc")]
    public class SpSyncAnchor
    {
        [XmlAttribute("pt")]
        public string PagingToken;
        
        [XmlAttribute("nt")]
        public string NextChangesToken;
        
        [XmlAttribute("p")]
        public int PageNumber;

        [XmlElement("nanc")]
        public SpSyncAnchor NextChangesAnchor;

        public SpSyncAnchor()
        { 
        
        }

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
            XmlSerializer ser = new XmlSerializer(typeof(SpSyncAnchor));
            MemoryStream s = new MemoryStream();
            ser.Serialize(s, anchor);
            return s.GetBuffer();
        }

        public static SpSyncAnchor GetAnchor(byte[] buffer)
        {
            if (buffer != null)
            {
                XmlSerializer ser = new XmlSerializer(typeof(SpSyncAnchor));
                MemoryStream s = new MemoryStream(buffer);
                return (SpSyncAnchor)ser.Deserialize(s);
            }
            else
            {
                return new SpSyncAnchor(null);
            }
        }
    }
}
