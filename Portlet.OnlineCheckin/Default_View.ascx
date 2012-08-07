<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Default_View.ascx.cs" Inherits="CUS.ICS.OnlineCheckin.Default_View" %>
<%@ Register TagPrefix="common" Namespace="Jenzabar.Common.Web.UI.Controls" Assembly="Jenzabar.Common" %>

<div id="divAdminLink" runat="server" visible="false">
	<table class="GrayBordered" width="50%" align="center" cellpadding="3" border="0">
		<tr>
			<td align="center"><IMG title="" alt="*" src="UI\Common\Images\PortletImages\Icons\portlet_admin_icon.gif">&nbsp;<common:globalizedlinkbutton OnClick="glnkAdmin_Click" id="glnkAdmin" runat="server" TextKey="TXT_CS_ADMIN_THIS_PORTLET"></common:globalizedlinkbutton></td>
		</tr>
	</table>
</div>
<asp:Panel ID="Checkin_Dialogue" runat="server" Enabled="false">
    <asp:Label ID="Instruction_Label" runat="server" Text="Label">Welcome to TLU's online checkin system!</asp:Label><br />
    <asp:CheckBox ID="Agree" runat="server" /><asp:Label ID="Label1" runat="server"
            Text="Label">I will be attending TLU in the upcoming semester.</asp:Label><br />
    <asp:Button ID="Submit" OnClick="checkin" runat="server" 
            Text="Check-In" />
</asp:Panel>