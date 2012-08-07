using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace CUS.ICS.OnlineCheckin.Model
{
    public class Hold
    {
        public String ID { get; private set; }
        public String Message { get; private set; }
        public SqlConnection DB;

        public Hold(String ID, String Message, SqlConnection DB)
        {
            this.ID = ID;
            this.Message = Message;
            this.DB = DB;
        }

        public static Hold from_ID(String ID, SqlConnection DB)
        {
            if (!DB.State.Equals(ConnectionState.Open))
                DB.Open();

            SqlCommand Command = new SqlCommand("dbo.CUST_get_hold", DB);
            Command.CommandType = System.Data.CommandType.StoredProcedure;
            Command.Parameters.Add("@ID", SqlDbType.Char);
            Command.Parameters["@ID"].Value = ID;
            SqlDataReader Result = Command.ExecuteReader();

            // case: We have no results!
            if (!Result.HasRows)
                return null; // Early return

            Result.Read();

            Hold New_Hold = new Hold((String)Result["ID"], (String)Result["MESSAGE"], DB);

            Result.Close();
            DB.Close();

            return New_Hold;
        }
    }
}