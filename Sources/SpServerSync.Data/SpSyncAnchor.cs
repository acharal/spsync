using System;
using Sp.Data.Caml;
using System.Xml.Serialization;

namespace Sp.Sync
{
    /// <summary>
    /// Holds the Sharepoint specific anchor of synchronization
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "S")]
    public class SpSyncAnchor
    {
        /// <summary>
        /// Sets or gets the token for the paging mechanism (PagingToken)
        /// </summary>
        [XmlElement(ElementName = "P")]
        public string PagingToken { set; get; }
        
        /// <summary>
        /// Sets or gets the token for the next synchronization (NextChangesToken)
        /// </summary>
        [XmlElement(ElementName = "N")]
        public string NextChangesToken { set; get; }

        /// <summary>
        /// Sets or gets a boolean if there are more changes after this anchor(HasMoreData)
        /// </summary>
        [XmlElement(ElementName = "H")]
        public bool HasMoreData = false;

        public static readonly SpSyncAnchor Empty = new SpSyncAnchor();

        /// <summary>
        /// Sets or gets the anchor for the next synchronization(NextChangesAnchor)
        /// </summary>
        [XmlElement(ElementName = "A")]
        public SpSyncAnchor NextChangesAnchor { set; get; }

        /// <summary>
        /// Initializes a new instance of SpSyncAnchor object
        /// </summary>
        public SpSyncAnchor()
        {
            NextChangesToken = ChangeBatch.FirstChangeBatch;
            PagingToken = ChangeBatch.FirstPage;
        }

        /// <summary>
        /// Initializes a new instance of SpSyncAnchor object
        /// </summary>
        /// <param name="changeToken">The token of the next synchronization</param>
        public SpSyncAnchor(string changeToken)
        {
            NextChangesToken = changeToken;
            PagingToken = ChangeBatch.FirstPage;
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
        }

    }
}
