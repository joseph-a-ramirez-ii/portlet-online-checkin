using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Jenzabar.Portal.Framework;
using Jenzabar.Portal.Framework.Web.UI;
using Jenzabar.Common.Web.UI.Controls;
using CUS.ICS.OnlineCheckin.Config;


namespace CUS.ICS.OnlineCheckin
{
    public partial class Admin_View : PortletViewBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SqlConnection DB = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["JenzabarConnectionString"].ConnectionString);
            
            Portlet_Config Config = Portlet_Config.from_DB(ref DB);

            IList<TextBoxEditor> Editors = new List<TextBoxEditor>();

            foreach (View_String Temp in Config.Values)
            {
                Panel Temp_Panel = new Panel();
                Label Temp_Label = new Label();
                TextBoxEditor Temp_Editor = new TextBoxEditor();

                Temp_Panel.ID = Temp.ID;
                Temp_Label.Text = Temp.Description;                
                Temp_Editor.InnerHtml = Temp.Body;

                Temp_Panel.Controls.Add(Temp_Label);
                Temp_Panel.Controls.Add(Temp_Editor);

                Main_Panel.Controls.Add(Temp_Panel);
            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            SqlConnection DB = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["JenzabarConnectionString"].ConnectionString);
            
            foreach (Panel Temp_Panel in Main_Panel.Controls.OfType<Panel>())
            {
                String Temp_ID = Temp_Panel.ID;
                String Temp_Description = Temp_Panel.Controls.OfType<Label>().First<Label>().Text;
                String Temp_Body = Temp_Panel.Controls.OfType<TextBoxEditor>().First<TextBoxEditor>().InnerHtml;

                View_String Temp_View_String = new View_String(Temp_ID, Temp_Description, Temp_Body);
                Temp_View_String.commit(ref DB);
                this.ParentPortlet.PreviousScreen();
            }
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            this.ParentPortlet.PreviousScreen();
        }
    }
}