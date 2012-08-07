using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace CUS.ICS.OnlineCheckin.Config
{
    public class View_String
    {
        private const String COMMIT_STORED_PROC = "CUST_OnlineCheckin$CommitCheckinString";

        public String ID { get; private set; }
        public String Description { get; set; }
        public String Body { get; set; }

        public View_String(String ID, String Description, String Body)
        {
            this.ID = ID;
            this.Description = Description;
            this.Body = Body;
        }

        public void commit(ref SqlConnection DB)
        {
            if (!DB.State.Equals(ConnectionState.Open))
                DB.Open();

            SqlCommand Command = new SqlCommand(COMMIT_STORED_PROC, DB);
            Command.CommandType = System.Data.CommandType.StoredProcedure;
            Command.Parameters.Add("@ID", SqlDbType.NVarChar);
            Command.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar);
            Command.Parameters.Add("@BODY", SqlDbType.NVarChar);
            SqlParameter SQL_Ret_Val = Command.Parameters.Add(new SqlParameter());
            SQL_Ret_Val.Direction = ParameterDirection.ReturnValue;

            Command.Parameters["@ID"].Value = ID;
            Command.Parameters["@DESCRIPTION"].Value = Description;
            Command.Parameters["@BODY"].Value = Body;
            Command.ExecuteReader();

            if ((int)SQL_Ret_Val.Value == (int)Error_Codes.NOT_FOUND)
                throw new Exception("Cannot update what does not exist");
            else if ((int)SQL_Ret_Val.Value != (int)Error_Codes.OK)
                throw new Exception("Unspecified error occured during execution of query");

            DB.Close();
        }
    }
}