
using System;
using System.Xml.Serialization;

namespace Sp.Sync.Data
{
    public class TypeMapping
    {
        public TypeMapping()
        { 
        
        }

        public TypeMapping(string fieldType, Type type, string sqlType)
        {
            FieldType = fieldType;
            Type = type;
            SqlType = sqlType;
        }

        public TypeMapping(string fieldType, Type type, string sqlType, int length)
            : this(fieldType, type, sqlType)
        {
            SqlLength = length;
        }

        public string FieldType;

        [XmlIgnore]
        public Type Type
        {
            get { return _actualType;  }
            set { _actualType = value; }
        }

        public string TypeName
        {
            get { return _actualType.FullName; }
            set { _actualType = System.Type.GetType(value); }
        }

        private Type _actualType;

        public string SqlType;
        
        public int? SqlLength;
    }
}
