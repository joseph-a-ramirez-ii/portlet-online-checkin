using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jenzabar.Portal.Framework;
using Jenzabar.Portal.Framework.Web.UI;
using CUS.ICS.OnlineCheckin;
using CUS.ICS.OnlineCheckin.Config;
using CUS.ICS.OnlineCheckin.Model;


namespace CUS.ICS.OnlineCheckin
{
    public partial class Default_View : PortletViewBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SqlConnection DB = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["JenzabarConnectionString"].ConnectionString);
            
            SqlCommand Command;

            String Feedback_String = "";

            if (PortalUser.Current.HostID == null || PortalUser.Current.HostID == "")
            {
                ParentPortlet.ShowFeedback("Something broke. Sorry. :(");
            }
            else if (ParentPortlet.AccessCheck("CanAdmin"))
            {
                Checkin_Dialogue.Visible = true;
                divAdminLink.Visible = true;
            }
            else
            {
                DB.Open();

                Command = new SqlCommand("dbo.CUST_User$Is_Student", DB);
                Command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter SQL_Input_ID = new SqlParameter("@ID", PortalUser.Current.HostID);
                SqlParameter SQL_Result = new SqlParameter("@RESULT", SqlDbType.Int);
                SQL_Result.Direction = ParameterDirection.Output;

                Command.Parameters.Add(SQL_Input_ID);
                Command.Parameters.Add(SQL_Result);
                
                Command.ExecuteScalar();

                DB.Close();

                // All of this to ask: Is this a student?
                if (SQL_Result.Value.Equals(DBNull.Value) || !Convert.ToBoolean(SQL_Result.Value)) {
                    ParentPortlet.ShowFeedback("You must be a student to view this page");
                } else {
                    bool eligible = true;

                    Command.CommandText = "dbo.Cust_Student$Is_Football";
                    DB.Open();
                    Command.ExecuteScalar();

                    if (Convert.ToBoolean(SQL_Result.Value)) {
                        eligible = false;
                        Checkin_Dialogue.Visible = false;
                        Feedback_String += "Football players may not check in online. Please contact your coach if you have any questions.<br />";
                    } else {
                        Command.CommandText = "dbo.Cust_Student$Is_Freshman_Commuter";
                        Command.ExecuteScalar();
                        if (Convert.ToBoolean(SQL_Result.Value)) {
                            SqlCommand Temp_Command = new SqlCommand("dbo.Cust_Student$Get_Groups", DB);
                            Temp_Command.CommandType = CommandType.StoredProcedure;

                            Temp_Command.Parameters.Add(new SqlParameter("@ID", PortalUser.Current.HostID));

                            SqlDataReader Temp_SQL_Results = Temp_Command.ExecuteReader();

                            List<String> Temp_List = new List<String>();

                            while (Temp_SQL_Results.Read())
                                Temp_List.Add((String)Temp_SQL_Results["GROUP_ID"]);

                            Temp_SQL_Results.Close();

                            if (!Temp_List.Contains("onlchk_ear")) {
                                eligible = false;
                                Agree.Visible = false;
                                Label1.Visible = false;
                                Submit.Visible = false;

                                Feedback_String += "You must check-in in-person.<br />";
                            }
                        } else {
                            Command.CommandText = "dbo.Cust_Student$Within_Checkin_Window";
                            Command.ExecuteScalar();

                            if (!Convert.ToBoolean(SQL_Result.Value)) {
                                eligible = false;
                                Feedback_String += "You may only check in during your designated window.<br />";
                            }
                        }
                        Command.CommandText = "dbo.Cust_Student$Is_Checked_In";
                        Command.ExecuteScalar();

                        if (Convert.ToBoolean(SQL_Result.Value)) {
                            eligible = false;
                            Agree.Checked = true;
                            Feedback_String += "You have checked in. Please check your email inbox for further information.";
                        }
                    }
                    Command.CommandText = "dbo.Cust_Student$Has_Holds";
                    Command.ExecuteScalar();
                    if (Convert.ToBoolean(SQL_Result.Value)) {
                        eligible = false;

                        Feedback_String += "You have one or more holds, please see below:<br />";
                        Feedback_String += "<ul>";

                        SqlCommand Temp_Command = new SqlCommand("dbo.CUST_Student$Get_Holds", DB);
                        Temp_Command.CommandType = CommandType.StoredProcedure;

                        Temp_Command.Parameters.Add(new SqlParameter("@ID", PortalUser.Current.HostID));

                        SqlDataReader Temp_Sql_Result = Temp_Command.ExecuteReader();

                        while (Temp_Sql_Result.Read())
                            Feedback_String += "<li> - " + Temp_Sql_Result["MESSAGE"] + "</li>";

                        Temp_Sql_Result.Close();

                        Feedback_String += "</ul>";
                    }
                    ParentPortlet.ShowFeedback(Feedback_String);
                    Update_Checkin_Panel(ref DB);

                    if (eligible)
                        Checkin_Dialogue.Enabled = true;
                    else
                        Checkin_Dialogue.Enabled = false;

                    DB.Close();
                    DB.Dispose();
                }
            }
        }
        protected void Update_Checkin_Panel(ref SqlConnection DB)
        {
            if (!DB.State.Equals(ConnectionState.Open))
                DB.Open();

            Portlet_Config Temp = Portlet_Config.from_DB(ref DB);

            SqlCommand Temp_Command = new SqlCommand("dbo.CUST_Student$Get_Groups", DB);
            Temp_Command.CommandType = CommandType.StoredProcedure;
            Temp_Command.Parameters.Add(new SqlParameter("@ID", PortalUser.Current.HostID));
            SqlDataReader Temp_Reader = Temp_Command.ExecuteReader();

            List<String> Temp_List = new List<String>();

            while (Temp_Reader.Read())
                Temp_List.Add((String)Temp_Reader["GROUP_ID"]);

            Temp_Reader.Close();

            if (Temp_List.Contains("onlchk_ear"))
                Instruction_Label.Text = Temp["pnl_msg_ear"].Body;
            else if (Temp_List.Contains("onlchk_con"))
                Instruction_Label.Text = Temp["pnl_msg_con"].Body;
            else if (Temp_List.Contains("onlchk_ftr")) {
                Temp_Command.CommandText = "dbo.CUST_Student$Is_Freshman_Commuter";

                SqlParameter Temp_SQL_Result = new SqlParameter("@RESULT", SqlDbType.Bit);
                Temp_SQL_Result.Direction = ParameterDirection.Output;

                Temp_Command.Parameters.Add(Temp_SQL_Result);

                Temp_Command.ExecuteScalar();
                
                if (Convert.ToBoolean(Temp_SQL_Result.Value))
                    Instruction_Label.Text = Temp["pnl_msg_Comm"].Body;
                else
                    Instruction_Label.Text = Temp["pnl_msg_ftr"].Body;
            }
        }

        protected void checkin(object sender, EventArgs e)
        {
            SqlConnection DB = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["JenzabarConnectionString"].ConnectionString);

            Student Current = Student.From_ID(Convert.ToInt32(PortalUser.Current.HostID), DB);
            if (Agree.Checked)
                // \o/
                try
                {
                    Current.check_in();
                }
                catch (Exception exc)
                {
                    ParentPortlet.ShowFeedback("This operation is not permitted: " + exc.Message);
                }
            Page_Load(sender, e);
        }

        protected void glnkAdmin_Click(object sender, EventArgs e)
        {
            this.ParentPortlet.NextScreen("Admin_View");
        }
    }
}