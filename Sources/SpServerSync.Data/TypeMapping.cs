using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpServerSync.Data
{
    public class TypeMapping
    {
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
        public Type Type;
        public string SqlType;
        public int? SqlLength;
    }
}
