using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sp.Data.Caml
{
    public static class ChangeCommands
    {
        public static readonly string Delete = "Delete";
        public static readonly string MoveAway = "MoveAway";
        public static readonly string InvalidToken = "InvalidToken";
        public static readonly string Restore = "Restore";
        public static readonly string SystemUpdate = "SystemUpdate";
        public static readonly string Rename = "Rename";


        static public bool IsDelete(string command)
        {
            return command == Delete || command == MoveAway;
        }
    }
}
