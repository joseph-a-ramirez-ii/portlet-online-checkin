using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using CUS.ICS.OnlineCheckin.Model;
using CUS.ICS.OnlineCheckin.Exceptions;



namespace CUS.ICS.OnlineCheckin.Model
{
    public class Student
    {
        private SqlConnection DB;
        
        public int id { get; private set; }
        public Dictionary<String, Hold> Holds { get; private set; }
        public Dictionary<String, Group> Groups { get; private set; }
        
        private Student(int id, Dictionary<String, Hold> Holds, Dictionary<String, Group>Groups, SqlConnection DB)
        {
            this.id = id;
            this.Holds = Holds;
            this.DB = DB;
            this.Groups = Groups;
        }

        public bool is_resident()
        {
            if (!DB.State.Equals(ConnectionState.Open))
                DB.Open();

            SqlCommand Command = new SqlCommand("dbo.CUS_Student$is_resident", DB);
            Command.CommandType = System.Data.CommandType.StoredProcedure;
            Command.Parameters.Add("@ID", SqlDbType.Int);
            SqlParameter SQL_Result = Command.Parameters.Add("@RESULT", SqlDbType.Bit);
            SQL_Result.Direction = ParameterDirection.Output;
            Command.Parameters["@ID"].Value = id;
            Command.ExecuteReader();
            DB.Close();

            return !DBNull.Value.Equals(SQL_Result.Value) && Convert.ToBoolean(SQL_Result.Value);
        }

        public static bool is_student(int id, SqlConnection DB)
        {
            if (!DB.State.Equals(ConnectionState.Open))
                DB.Open();

            SqlCommand Command = new SqlCommand("dbo.CUS_is_student", DB);
            Command.CommandType = System.Data.CommandType.StoredProcedure;
            Command.Parameters.Add("@ID", SqlDbType.Int);
            SqlParameter SQL_Result = Command.Parameters.Add("@RESULT", SqlDbType.Bit);
            SQL_Result.Direction = ParameterDirection.Output;
            Command.Parameters["@ID"].Value = id;
            Command.ExecuteReader();
            DB.Close();

            return !DBNull.Value.Equals(SQL_Result.Value) && Convert.ToBoolean(SQL_Result.Value);
        }

        public static Student From_ID(int id, SqlConnection DB) 
        {
            if (!DB.State.Equals(ConnectionState.Open))
                DB.Open();

            SqlCommand Command = new SqlCommand("dbo.CUS_get_Students",DB);
            Command.CommandType = System.Data.CommandType.StoredProcedure;
            Command.Parameters.Add("@ID", SqlDbType.Int);
            Command.Parameters["@ID"].Value = id;
            SqlDataReader Result = Command.ExecuteReader();

            // case: We have no results!
            if (!Result.HasRows)
                return null; // Early return
            
            Result.Read();
            
            // Put holds into a list
            Dictionary<String, Hold> Holds = new Dictionary<String, Hold>();
            IList<String> Hold_Codes = new List<String>();

            if (!DBNull.Value.Equals(Result["HOLD_1_CDE"]))
                Hold_Codes.Add((String)Result["HOLD_1_CDE"]);
            if (!DBNull.Value.Equals(Result["HOLD_2_CDE"]))
                Hold_Codes.Add((String)Result["HOLD_2_CDE"]);
            if (!DBNull.Value.Equals(Result["HOLD_3_CDE"]))
                Hold_Codes.Add((String)Result["HOLD_3_CDE"]);
            if (!DBNull.Value.Equals(Result["HOLD_4_CDE"]))
                Hold_Codes.Add((String)Result["HOLD_4_CDE"]);
            if (!DBNull.Value.Equals(Result["HOLD_5_CDE"]))
                Hold_Codes.Add((String)Result["HOLD_5_CDE"]);
            if (!DBNull.Value.Equals(Result["HOLD_6_CDE"]))
                Hold_Codes.Add((String)Result["HOLD_6_CDE"]);

            Result.Close();

            if (Hold_Codes.Count > 0)
                foreach (String Temp in Hold_Codes)
                    Holds.Add(Temp, Hold.from_ID(Temp, DB));

            Command = new SqlCommand("dbo.CUS_get_groups",DB);
            Command.CommandType = System.Data.CommandType.StoredProcedure;
            Command.Parameters.Add("@ID", SqlDbType.Int);
            Command.Parameters["@ID"].Value = id;

            if (!DB.State.Equals(ConnectionState.Open))
                DB.Open();

            Result = Command.ExecuteReader();

            if (!Result.HasRows)
                throw new Exception("Student is not part of any groups");

            Dictionary<String, Group> Groups = new Dictionary<String, Group>();

            while (Result.Read())
            {
                String Group_ID = (String)Result["GROUP_ID"];
                Group_ID = Group_ID.Trim();
                Groups.Add(Group_ID, new Group(Group_ID, (DateTime)Result["BEGIN_WINDOW"], (DateTime)Result["END_WINDOW"]));
            }

            Result.Close();
            DB.Close();

            // That's no moon... That's a space station...
            return new Student(id, Holds, Groups, DB);

        }

        //Not implemented
        public Student create_student()
        {
            return null;
        }

        public bool is_checked_in()
        {
            if (!DB.State.Equals(ConnectionState.Open))
                DB.Open();

            SqlCommand Command = new SqlCommand("CUST_OnlineCheckin$isCheckedIn", DB);
            Command.CommandType = System.Data.CommandType.StoredProcedure;
            Command.Parameters.Add("@ID_NUM", SqlDbType.Int);
            SqlParameter SQL_Result = Command.Parameters.Add("@RESULT", SqlDbType.Bit);
            SQL_Result.Direction = ParameterDirection.Output;
            Command.Parameters["@ID_NUM"].Value = id;
            Command.ExecuteReader();

            if (SQL_Result.Value == null)
                throw new Exception("No values returned from procedure.");

            DB.Close();
            return Convert.ToBoolean(SQL_Result.Value);
        }

        public bool has_holds()
        {
            return Holds.Count > 0;
        }

        public bool within_window()
        {
            DateTime Begin = Groups.Values.First<Group>().Begin_Window;
            DateTime End = Groups.Values.First<Group>().End_Window;
            foreach (Group item in Groups.Values)
            {
                if (Begin == null)
                    Begin = item.Begin_Window;
                else if (item.Begin_Window != null && item.Begin_Window.CompareTo(Begin) < 0)
                    Begin = item.Begin_Window;
                if (End == null)
                    End = item.End_Window;
                else if (item.Begin_Window != null && item.End_Window.CompareTo(End) > 0)
                    End = item.End_Window;
            }

            if (Begin == null || End == null)
                throw new Exception("This \"student\" has no checkin window");
            
            return Begin.CompareTo(DateTime.Now) <= 0 && DateTime.Now.CompareTo(End) <= 0;
        }

        public void check_in()
        {
            // Goes through the steps to check the student in. Make sure the student is eligible to be checked in!
            if (is_checked_in())
                throw new Exception("Student is already checked in");
            if (has_holds())
                throw new Exception("Student has a hold");
            if (!within_window())
                throw new Exception("Outside of student's registration window");

            if (!DB.State.Equals(ConnectionState.Open))
                DB.Open();

            SqlCommand Command = new SqlCommand("dbo.CUST_OnlineCheckin$Checkin", DB);
            Command.CommandType = System.Data.CommandType.StoredProcedure;
            Command.Parameters.Add("@ID_NUM", SqlDbType.Int);
            Command.Parameters["@ID_NUM"].Value = id;
            Command.ExecuteReader();

            DB.Close();
         }
    }
}