﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace SpServerSync.Data
{
    public class SpSyncTableAnchorCollection : Collection<SpSyncTableAnchor>
    {

        public SpSyncTableAnchorCollection()
        {

        }

        public bool Contains(string tableName)
        {
            foreach (SpSyncTableAnchor tableAnchor in this)
                if (tableAnchor.TableName == tableName)
                    return true;
            return false;
        }

        public SpSyncAnchor this[string tableName]
        { 
            get {
                foreach (SpSyncTableAnchor tableAnchor in this)
                    if (tableAnchor.TableName == tableName)
                        return tableAnchor.Anchor;

                return null;
            }

            set
            {
                foreach (SpSyncTableAnchor tableAnchor in this)
                {
                    if (tableAnchor.TableName == tableName)
                    {
                        tableAnchor.Anchor = value;
                        return;
                    }
                }

                SpSyncTableAnchor newAnchor = new SpSyncTableAnchor();
                newAnchor.TableName = tableName;
                newAnchor.Anchor = value;
                this.Add(newAnchor);
            }
        }

        public static SpSyncTableAnchorCollection Deserialize(byte[] buffer)
        {
            using (MemoryStream m = new MemoryStream(buffer))
            {
                var ser = new XmlSerializer(typeof(SpSyncTableAnchorCollection));
                object o = ser.Deserialize(m);
                return o as SpSyncTableAnchorCollection;
            }
        }

        public static byte[] Serialize(SpSyncTableAnchorCollection x)
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
