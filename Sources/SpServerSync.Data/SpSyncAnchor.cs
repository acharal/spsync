using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace SpServerSync.Data
{
    /// <summary>
    /// Holds the Sharepoint specific anchor of synchronization
    /// </summary>
    [Serializable]
    public class SpSyncAnchor
    {
        /// <summary>
        /// Sets or gets the token for the paging mechanism
        /// </summary>
        public string PagingToken { set; get; }
        
        /// <summary>
        /// Sets or gets the token for the next synchronization
        /// </summary>
        public string NextChangesToken { set; get; }

        /// <summary>
        /// Sets or gets the number of the page
        /// </summary>
        public int PageNumber { set; get; }

        /// <summary>
        /// Sets or gets the anchor for the next synchronization
        /// </summary>
        public SpSyncAnchor NextChangesAnchor { set; get; }

        /// <summary>
        /// Initializes a new instance of SpSyncAnchor object
        /// </summary>
        public SpSyncAnchor()
        { 
        
        }

        /// <summary>
        /// Initializes a new instance of SpSyncAnchor object
        /// </summary>
        /// <param name="changeToken">The token of the next synchronization</param>
        public SpSyncAnchor(string changeToken)
        {
            NextChangesToken = changeToken;
            PagingToken = null;
            PageNumber = 1;
        }

        /// <summary>
        /// Initializes a new instance of SpSyncAnchor object
        /// </summary>
        /// <param name="changeToken">The token of the next synchronization</param>
        /// <param name="pageToken">The token of the next page in the current synchronization</param>
        public SpSyncAnchor(string changeToken, string pageToken)
        {
            NextChangesToken = changeToken;
            PagingToken = pageToken;
            PageNumber = 1;
        }

        /// <summary>
        /// Initializes a new instance of SpSyncAnchor object
        /// </summary>
        /// <param name="changeToken">The token of the next synchronization</param>
        /// <param name="pageToken">The token of the next page in the current synchronization</param>
        /// <param name="num">The number of the current page</param>
        public SpSyncAnchor(string changeToken, string pageToken, int num)
        {
            NextChangesToken = changeToken;
            PagingToken = pageToken;
            PageNumber = num;
        }

        /*
        /// <summary>
        /// Serializes an SpSyncAnchor object to a sequence of bytes
        /// </summary>
        /// <param name="anchor">The SpSyncAnchor object</param>
        /// <returns>a sequence of bytes representing the serialized object</returns>
        public static byte[] GetBytes(SpSyncAnchor anchor)
        {
            XmlSerializer ser = new XmlSerializer(typeof(SpSyncAnchor));
            MemoryStream s = new MemoryStream();
            ser.Serialize(s, anchor);
            return s.GetBuffer();
        }

        /// <summary>
        /// Deserializes an SpSyncAnchor object from an array of bytes
        /// </summary>
        /// <param name="buffer">the serialized object as an array of bytes</param>
        /// <returns>the instance of the deserialized object or null if deserialization failed</returns>
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
        */
    }
}
