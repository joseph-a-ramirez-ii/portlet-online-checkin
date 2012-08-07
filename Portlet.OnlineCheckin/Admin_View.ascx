<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Admin_View.ascx.cs" Inherits="CUS.ICS.OnlineCheckin.Admin_View" %>
<%@ Register TagPrefix="jenzabar" Namespace="Jenzabar.Common.Web.UI.Controls" Assembly="Jenzabar.Common" %>

<asp:Panel ID="Main_Panel" runat="server">
</asp:Panel>
<asp:Button ID="Save" runat="server" Text="Save" OnClick="Save_Click" />
<asp:Button ID="Cancel" runat="server" Text="Cancel" OnClick="Back_Click" />