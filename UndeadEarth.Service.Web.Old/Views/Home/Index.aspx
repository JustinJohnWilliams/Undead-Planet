<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        var gameInstance;
        var lastActionElement = null;
        function ActionCompleted(methodName) {
            ShowCommandConfirmed(lastActionElement);
        }

        function GetUserDescription(user, veShape) {
            if (user.CanHunt) {
                if (user.HotZone.IsDestroyed == false) {
                    return "Zombies Left: " + user.HotZone.ZombiesLeft + " out of " + user.HotZone.MaxZombies + "<br/>" +
                        user.HotZone.RegenRate + " more zombies in " + user.HotZone.MinutesToNextRegen + " minutes <br/>" +
                        "<a href='#' onclick='javascript:Hunt(this);'>hunt for zombies!</a><br/>";
                }
                else {
                    return '<strong>Hot Zone Destroyed!</strong>';
                }
            }
            else {
                return '';
            }
        }

        function GetHotZoneDescription(hotZone, veShape) {
            var description =
                   "ETA: " + hotZone.MinutesAway + " minutes<br/>" +
                   "Miles Away: " + hotZone.MilesAway + "<br/>";
            if (hotZone.IsDestroyed == false) {
                description +=
                    "Zombies Left: " + hotZone.ZombiesLeft + " out of " + hotZone.MaxZombies + "<br/>" +
                    hotZone.RegenRate + " more zombies in " + hotZone.MinutesToNextRegen + " minutes <br/>" +
                    "<a href='#' id='" + hotZone.Id + "' onclick='javascript:MoveToHotZone(this);'>move here</a>";
            }
            else {
                description += '<strong>Hot Zone Destoyed!</strong>';
            }

            return description;
        }

        function GetInfoNodeDescription(infoNode, veShape) {
            var description = 
                "ETA: " + infoNode.MinutesAway + " minutes<br/>Miles Away: " + infoNode.MilesAway + "<br/>"
                + "<a href='#' id='" + infoNode.Id + "' onclick='javascript:MoveToInfoNode(this);'>move here</a><br/>";

            return description;
        }

        function SetUserStatus(data) {
            $('#displayName').html(data.Name);
            if (data.IsMoving == false) {
                var description = 'You are currently not moving.<br/>';
                if (data.CanHunt == true) {
                    description = description + "<br/>Hot Zone Info:<br/>";
                    if (data.HotZone.IsDestroyed == false) {
                        description = description + "Zombies Left: " + data.HotZone.ZombiesLeft + " out of " + data.HotZone.MaxZombies + "<br/>" +
                        data.HotZone.RegenRate + " more zombies in " + data.HotZone.MinutesToNextRegen + " minutes <br/>" +
                        "<a href='#' onclick='javascript:Hunt(this);'>hunt for zombies!</a><br/>";
                    }
                    else {
                        description = description + '<br/>Hot Zone Destroyed!';
                    }
                }

                $('#status').html(description);
            }
            else {
                $('#status').html(data.MinutesLeftInMove
                                  + " minutes till destination reached, going "
                                  + data.CurrentSpeed
                                  + " mph. Destination: <a href='#' onclick='javascript:gameInstance.CenterMap("
                                  + data.NextDestinationLatitude 
                                  + ","
                                  + data.NextDestinationLongitude
                                  + ", 18);'>"
                                  + data.NextNodeName
                                  + "</a>.");
            }
        }
        
        function ready() {
            gameInstance = new Game();
            gameInstance.GetInfoNodeDescription = GetInfoNodeDescription;
            gameInstance.GetHotZoneDescription = GetHotZoneDescription;
            gameInstance.GetUserDescription = GetUserDescription;
            gameInstance.OnActionCompleted = ActionCompleted;
            gameInstance.SetUserStatus = SetUserStatus;
            gameInstance.Start();
        }

        function MoveToHotZone(element) {
            lastActionElement = element;
            gameInstance.MoveToHotZone($(element).attr('id'));
        }

        function MoveToInfoNode(element) {
            lastActionElement = element;
            gameInstance.MoveToInfoNode($(element).attr('id'));
        }

        function Hunt(element) {
            lastActionElement = element;
            gameInstance.Hunt();
        }
        
        $(ready);
    </script>
    
    <!-- map info -->
    <%= Html.Hidden("mapPlaceHolderId", "map") %>
    
    <!-- user specific -->
    <%= Html.Hidden("userIconUrl", Url.Content("~/content/images/you.png")) %>
    <%= Html.Hidden("getUserControllerAction", Url.RouteUrl(new { controller = "Home", action = "GetUser" })) %>
    
    <!-- hot zones -->
    <%= Html.Hidden("hotZoneIconUrl", Url.Content("~/content/images/hunt.png")) %>
    <%= Html.Hidden("getHotZonesControllerAction", Url.RouteUrl( new { controller = "Home", action = "GetHotZones"})) %>
    <%= Html.Hidden("getHotZoneControllerAction", Url.RouteUrl(new { controller = "Home", action = "GetHotZone" }))%>
    <%= Html.Hidden("moveToHotZoneControllerAction", Url.RouteUrl(new { controller = "Home", action = "MoveToHotZone" })) %>
    <%= Html.Hidden("huntControllerAction", Url.RouteUrl(new { controller = "Home", action = "Hunt"})) %>
        
    <!-- info nodes -->
    <%= Html.Hidden("infoNodeIconUrl", Url.Content("~/content/images/info.png")) %>
    <%= Html.Hidden("getInfoNodesControllerAction", Url.RouteUrl(new { controller = "Home", action = "GetInfoNodes"})) %>
    <%= Html.Hidden("getInfoNodeControllerAction", Url.RouteUrl(new { controller = "Home", action = "GetInfoNode"})) %>
    <%= Html.Hidden("moveToInfoNodeControllerAction", Url.RouteUrl(new { controller = "Home", action = "MoveToInfoNode" }))%>
    
    <div id='map' style="position: relative; width: 100%; height: 400px;">
    </div>
    <div id="commandConfirmed" 
          class="ui-state-highlight ui-corner-all" 
          style="padding: 5px; margin-top: 10px; font-size: 1.5em; float: right; display: none;" >
        Command Confirmed...
    </div>
    <h2 id="displayName">Loading...</h2> 
    
    <div id="status" style="font-size: 1.4em;">
        Loading...
    </div>
</asp:Content>
