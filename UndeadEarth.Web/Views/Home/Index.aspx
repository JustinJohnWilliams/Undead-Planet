<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<UndeadEarth.Controllers.ViewModels.HomePageViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            if (top.location.toString().toLowerCase().indexOf("undeadearththegame.com") > 1) {
                top.location = "http://apps.facebook.com/undeadplanet";
            }
        });
    </script>


    <h2>Undead Planet</h2>
    <div style="height: 1000px">
        <% 
            string inviteUrl = Url.RouteUrl(new { controller = "Facebook", action = "Invite" });
            string playUrl = Url.RouteUrl(new { controller = "Home", action = "Game" });
            string registerUrl = Url.RouteUrl(new { controller = "Facebook", action = "Register" }); 
        %>
        <% if(ViewData.Model.IsRegistered == true) { %>
            <a id="hrefPlayNow" href="<%= playUrl %>">Play Now</a>
        <% } else { %>
            <a id="A1" href="<%= registerUrl %>">Play Now</a>
        <% } %>

        <a href="<%= inviteUrl %>" style="margin-left: 20px;" >Invite Your Friends</a>
        <h3>Rankings</h3>
        <% 
            int rank = 1;
            foreach(var user in ViewData.Model.UserRank) { %>
            <div class="ui-widget-content ui-corner-all" style="padding: 5px">
                <span style="font-size: 1.3em">#<%= rank.ToString() %>.</span><br />
                Name: <%= user.Name %><br />
                Level: <%= user.Level %><br />
                Hot Zones Destroyed: <%= user.HotZonesDestroyed %><br />
                Hoards Destroyed: <%= user.ZombiePacksDestroyed %><br />
                Zombies Killed: <%= user.ZombiesKilled %><br />
                Money Earned: <%= user.MoneyAccumulated %><br />
                Miles Traveled: <%= user.MilesTraveled %><br />
                Kill Streak: <%= user.KillStreak %><br />
            </div>
            <br />
            <br />
        <% rank += 1; } %>
    </div>
</asp:Content>
