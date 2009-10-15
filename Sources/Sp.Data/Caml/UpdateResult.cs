using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sp.Data.Caml
{
    public class UpdateResult
    {
        public int UpdateItemID;
        public string Command;
        public string ErrorCode;
        public string ErrorMessage;
        public ListItem ItemData;

        public static readonly string SuccessCode = "0x00000000";
        
        public static readonly string VersionConflict = "0x81020015";

        public static readonly string ItemDeleted = "missingcode";

        public bool IsSuccess()
        {
            return ErrorCode == SuccessCode;
        }
    }
}
