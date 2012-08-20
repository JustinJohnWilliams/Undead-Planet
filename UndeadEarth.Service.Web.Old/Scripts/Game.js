var _game;

function Game() {
    
    if (_game != null) throw 'Only one instance of the game can exist at any point in time.';
    _game = this;

    //map
    this.Map = new VEMap($('#mapPlaceHolderId').val());
    this.Map.AttachEvent('onmouseover', this.GetNodeInfo);

    //user
    this.UserNode = null;
    this.UserIconUrl = $('#userIconUrl').val();
    this.GetUserControllerAction = $('#getUserControllerAction').val();
    
    //hot zone
    this.HotZoneIconUrl = $('#hotZoneIconUrl').val();
    this.GetHotZonesControllerAction = $('#getHotZonesControllerAction').val();
    this.GetHotZoneControllerAction = $('#getHotZoneControllerAction').val();
    this.MoveToHotZoneControllerAction = $('#moveToHotZoneControllerAction').val();
    this.HuntControllerAction = $('#huntControllerAction').val();

    //info nodes
    this.InfoNodeIconUrl = $('#infoNodeIconUrl').val();
    this.GetInfoNodesControllerAction = $('#getInfoNodesControllerAction').val();
    this.GetInfoNodeControllerAction = $('#getInfoNodeControllerAction').val();
    this.MoveToInfoNodeControllerAction = $('#moveToInfoNodeControllerAction').val();
}

Game.prototype.GetNodeInfo = function(eventArgs) {
    if (eventArgs.elementID != null) {
        var shape = _game.Map.GetShapeByID(eventArgs.elementID);
        if (shape.NodeType == "HotZone") {
            $.getJSON(_game.GetHotZoneControllerAction,
                    { hotZoneId: shape.NodeId },
                    function(data) {
                        shape.SetDescription(_game.GetHotZoneDescription(data, shape));
                    });
        }
        else if (shape.NodeType == "InfoNode") {
            $.getJSON(_game.GetInfoNodeControllerAction,
                    { infoNodeId: shape.NodeId },
                    function(data) {
                        shape.SetDescription(_game.GetInfoNodeDescription(data, shape));
                    });
        }
        else if (shape.NodeType == "User") {
            this.UpdateUser();
        }
    }
}

Game.prototype.Start = function() {
    $.getJSON(this.GetUserControllerAction,
        function(data) {
            var userPosition = new VELatLong(data.Latitude, data.Longitude, 0, VEAltitudeMode.RelativeToGround);
            _game.Map.LoadMap(userPosition, 13, VEMapStyle.Road, false, VEMapMode.Mode2D, false, 0);
            _game.UserNode = new VEShape(VEShapeType.Pushpin, userPosition);
            _game.UserNode.SetZIndex(9001); //over 9000?!
            _game.UserNode.SetTitle(data.Name);
            _game.UserNode.SetCustomIcon(_game.UserIconUrl);
            _game.UserNode.SetPoints(userPosition);
            _game.UserNode.NodeType = "User";
            _game.Map.AddShape(_game.UserNode);
            _game.GetHotZones();
            _game.GetInfoNodes();
            setInterval('_game.UpdateUser();', 3000);
        });
}

Game.prototype.UpdateUser = function() {
    $.getJSON(this.GetUserControllerAction, this.OnUpdateUser);
};

Game.prototype.OnUpdateUser = function(data) {
    var userPosition = new VELatLong(data.Latitude, data.Longitude, 0, VEAltitudeMode.RelativeToGround);
    _game.UserNode.SetDescription(_game.GetUserDescription(data, _game.UserNode));
    _game.UserNode.SetPoints(userPosition);
    _game.SetUserStatus(data);
};

Game.prototype.CenterMap = function(latitude, longitude, zoom) {
    if (latitude != null && longitude != null) {
        this.Map.SetCenterAndZoom(new VELatLong(latitude, longitude, 0, VEAltitudeMode.RelativeToGround), zoom);
    }
}

Game.prototype.MoveToHotZone = function(hotZoneId) {
    $.post(this.MoveToHotZoneControllerAction, { hotZoneId: hotZoneId }, this.OnActionCompleted);
}

Game.prototype.MoveToInfoNode = function(infoNodeId) {
    $.post(this.MoveToInfoNodeControllerAction, { infoNodeId: infoNodeId }, this.OnActionCompleted);
}

Game.prototype.GetHotZones = function() {
    $.getJSON(this.GetHotZonesControllerAction, this.OnGetHotZones);
}

Game.prototype.Hunt = function() {
    $.post(this.HuntControllerAction, null, function() { _game.OnActionCompleted('Hunt'); });
}

Game.prototype.OnGetHotZones = function(data) {
    for (index = 0; index < data.length; index++) {
        var latLong = new VELatLong(data[index].Latitude, data[index].Longitude, 0, VEAltitudeMode.RelativeToGround);
        var shape = new VEShape(VEShapeType.Pushpin, latLong);
        shape.SetTitle(data[index].Name);
        shape.SetDescription(_game.GetHotZoneDescription(data[index], shape));
        shape.SetCustomIcon(_game.HotZoneIconUrl);
        shape.SetMinZoomLevel(7);
        shape.NodeId = data[index].Id;
        shape.NodeType = "HotZone";
        _game.Map.AddShape(shape);
    }
}

Game.prototype.GetInfoNodes = function() {
    $.getJSON(this.GetInfoNodesControllerAction, this.OnGetInfoNodes);
}

Game.prototype.OnGetInfoNodes = function(data) {
    for (index = 0; index < data.length; index++) {
        var latLong = new VELatLong(data[index].Latitude, data[index].Longitude, 0, VEAltitudeMode.RelativeToGround);
        var shape = new VEShape(VEShapeType.Pushpin, latLong);
        shape.SetTitle(data[index].Name);
        shape.SetDescription(_game.GetInfoNodeDescription(data[index], shape));
        shape.SetCustomIcon(_game.InfoNodeIconUrl);
        shape.NodeId = data[index].Id;
        shape.NodeType = "InfoNode";
        _game.Map.AddShape(shape);
    }
}