<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Invite
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Invite</h2>
    <% string homeUrl = Url.RouteUrl(new { controller = "Home", action = "Index" }); %>
    <a href="<%= homeUrl %>">Back</a>
    <br />
    <% if((bool)ViewData["Success"] == true) { %>
        <div class="ui-state-highlight ui-corner-all" style="padding: 10px; font-size: 1.2em" >
            You friends have been invited!  <a href="<%= homeUrl %>">Click here to return to Home.</a>
        </div>
        <br />
    <% } %>

    <% Html.RenderPartial("~/Views/Facebook/FacebookFriendInviteCondensed.ascx"); %>
</asp:Content>
