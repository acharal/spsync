using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sp.Data.Caml
{
    public class ListItem
    {
        public int ID { set; get; }
        public IList<KeyValuePair<string, string>> Fields { set; get; }

        public string this[string field]
        {
            get { 
                KeyValuePair<string,string> kvp = Fields.Where( k => k.Key == field).FirstOrDefault();
                return (!kvp.Equals(default(KeyValuePair<string,string>))) ? kvp.Value : null;
            }
        }
    }
}
