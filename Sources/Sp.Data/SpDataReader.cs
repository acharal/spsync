using System;
using System.Data;
using System.Xml;
using System.Collections.Generic;

namespace Sp.Data
{
    public class SpDataReader : IDataReader
    {
        private XmlReader reader;

        // private IDictionary<string, string> currentRecord;

        DataTable dt = new DataTable();

        public SpDataReader(XmlReader xmlReader)
        {
            reader = xmlReader;
            dt.ReadXml(xmlReader);
        }

        #region IDataReader Members

        public void Close()
        {
            reader.Close();
        }

        public int Depth
        {
            get { throw new NotImplementedException(); }
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool IsClosed
        {
            get {
                return reader.ReadState == ReadState.Closed;
            }
        }

        public bool NextResult()
        {
            return true;
        }

        public bool Read()
        {
            try
            {
                var currentRecord = ParseRecord();

                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }

        public int RecordsAffected
        {
            get { return 0; }
        }

        private IDictionary<string,string> ParseRecord()
        {
            Dictionary<string, string> record = new Dictionary<string, string>();
            
            reader.ReadStartElement("row", "z");

            reader.MoveToFirstAttribute();

            int lengthToSkip = "ows_".Length;

            if (reader.HasAttributes)
            {
                // reader.HasAttributes
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name.StartsWith("ows_"))
                        record[reader.Name.Substring(lengthToSkip)] = reader.Value;
                }
            }

            reader.ReadEndElement();

            return record;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDataRecord Members

        public int FieldCount
        {
            get { throw new NotImplementedException(); }
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public object GetValue(int i)
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        public object this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public object this[int i]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
