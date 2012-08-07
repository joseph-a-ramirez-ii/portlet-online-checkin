using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace CUS.ICS.OnlineCheckin.Model
{
    public class Group
    {
        private String ID;
        public DateTime Begin_Window { get; private set; }
        public DateTime End_Window { get; private set; }

        public Group(String ID, DateTime Begin_Window, DateTime End_Window)
        {
            this.ID = ID;
            this.Begin_Window = Begin_Window;
            this.End_Window = End_Window;
        }

        public static Group From_ID(int id, SqlConnection DB)
        {
            return null;
        }
    }
}