<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<UndeadEarth.Controllers.ViewModels.StartingHotZoneViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Register
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        string registerUrl = Url.RouteUrl(new { controller = "Facebook", action = "Register" });
    %>
    <h2>Pick Your Starting Location</h2>
    <form action="<%= registerUrl %>" method="post">
        <%= Html.DropDownList("startingHotZoneId", ViewData.Model.HotZones) %>
        <input type="submit" value="begin" />
    </form>
</asp:Content>
