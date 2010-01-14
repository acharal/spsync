using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sp.Data.Caml
{
    public class ChangeLog : List<ChangeItem>
    {
        public ChangeLog(IEnumerable<ChangeItem> changes)
            : base(changes) { }

        public bool MoreChanges;
        
        public string NextLastChangeToken;

        public ListDef NewListDef;

        public bool InvalidToken
        {
            get { return base.Count == 1 && base[0].Command == "InvalidToken"; }
        }
    }
}
