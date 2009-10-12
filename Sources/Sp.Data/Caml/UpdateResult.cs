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

        private static readonly string SuccessCode = "0x00000000";

        public bool IsSuccess()
        {
            return ErrorCode == SuccessCode;
        }
    }
}
