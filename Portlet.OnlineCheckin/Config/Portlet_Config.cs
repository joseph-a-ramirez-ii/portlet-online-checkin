using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace CUS.ICS.OnlineCheckin.Config
{
    public class Portlet_Config : Dictionary<String, View_String>
    {
        public static Portlet_Config from_DB(ref SqlConnection DB)
        {
            if (!DB.State.Equals(ConnectionState.Open))
                DB.Open();
            
            SqlCommand Command = new SqlCommand("dbo.CUST_OnlineCheckin$Get_Strings", DB);
            Command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader SQL_Result = Command.ExecuteReader();

            Portlet_Config Temp = new Portlet_Config();

            while (SQL_Result.Read())
                Temp.Add((String)SQL_Result["ID"] ,new View_String((String)SQL_Result["ID"], (String)SQL_Result["Description"], (String)SQL_Result["Body"]));

            SQL_Result.Close();

            return Temp;
        }
    }
}