using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CUS.ICS.OnlineCheckin;
using CUS.ICS.OnlineCheckin.Model;
using CUS.ICS.OnlineCheckin.Config;
using System.Data;
using System.Data.SqlClient;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, world!");
            
            //Connect to the database here. ;)

            //SqlCommand Temp_Command = new SqlCommand("dbo.CUST_Student$Is_Freshman_Commuter", DB);
            //Temp_Command.CommandType = CommandType.StoredProcedure;

            //SqlParameter ID = new SqlParameter("@ID", 30122726);
            //SqlParameter Result = new SqlParameter("@RESULT", SqlDbType.Bit);
            //Result.Direction = ParameterDirection.Output;

            //Temp_Command.Parameters.Add(ID);
            //Temp_Command.Parameters.Add(Result);
            
            //DB.Open();

            //SqlDataReader Table = Temp_Command.ExecuteReader();

            //Console.WriteLine(Convert.ToBoolean(Result.Value));

            //Table.Close();

            //Portlet_Config Config = Portlet_Config.from_DB(ref DB);

            //Console.WriteLine(Config["pnl_msg_Comm"].Body);

            //Console.ReadLine();
        }
    }
}
