<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Redirect
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    Redirecting...please wait

    <script type="text/javascript">
        top.location = "https://graph.facebook.com/oauth/authorize?client_id=115289678504285&redirect_uri=http://apps.facebook.com/undeadplanet/&type=user_agent&display=page";
    </script>

</asp:Content>
