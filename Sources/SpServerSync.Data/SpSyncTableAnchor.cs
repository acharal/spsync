using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sp.Sync.Data
{
    public class SpSyncTableAnchor
    {
        [XmlElement(ElementName = "S")]
        public string SiteName;

        [XmlElement(ElementName = "L")]
        public string TableName;

        [XmlElement(ElementName = "A")]
        public SpSyncAnchor Anchor;
    }
}
