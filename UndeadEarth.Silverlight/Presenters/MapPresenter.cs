using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using UndeadEarth.Model.Proxy;
using Microsoft.Maps.MapControl;
using UndeadEarth.Silverlight.Model;

namespace UndeadEarth.Silverlight.Presenters
{
    public class MapPresenter
    {
        public interface IView
        {
            /// <summary>
            /// Delegates to the view to display the stores
            /// </summary>
            /// <param name="stores"></param>
            void AddStores(List<StoreNode> stores);

            /// <summary>
            /// Delegates to the view to remove stores after they are out of radius
            /// </summary>
            /// <param name="stores"></param>
            void RemoveStores(List<StoreNode> stores);

            /// <summary>
            /// Delegates to the view to display the safe houses
            /// </summary>
            /// <param name="safeHouses"></param>
            void AddSafeHouses(List<SafeHouseNode> safeHouses);

            /// <summary>
            /// Delegates to the view to remove safe houses after they are out of radius
            /// </summary>
            /// <param name="safeHouses"></param>
            void RemoveSafeHouses(List<SafeHouseNode> safeHouses);

            /// <summary>
            /// Delegates to the view to add zombie packs.
            /// </summary>
            void AddZombiePacks(List<Node> zombiePacks);

            /// <summary>
            /// Delegates to teh iew to remove zombie packs.
            /// </summary>
            void RemoveZombiePacks(List<Guid> zombiePacks);

            /// <summary>
            /// Delegates to the view to create the user.
            /// </summary>
            void InitializeUser(UserNode userNode);

            /// <summary>
            /// Delegates to the view to move to current user's location.
            /// </summary> 
            void MoveToUser(int zoomLevel);

            /// <summary>
            /// Delegates to the view to updated the user's location.
            /// </summary>
            void UpdateUserLocation(double latitude, double longitude);

            /// <summary>
            /// Delegates to the view to popluates user's move radius.
            /// </summary>
            /// <param name="radiusInMiles"></param>
            void UpdateUserMoveRadius(List<Location> locations);

            /// <summary>
            /// Tells the view to update a hotzone's cleared status.
            /// </summary>
            void UpdateHotZoneClearedStatus(Guid hotZoneId, bool isCleared);

            /// <summary>
            /// Delegates to the view to updated sight radius for user.
            /// </summary>
            /// <param name="locations"></param>
            //void UpdateSightRadius(List<Location> locations);

            /// <summary>
            /// Delegates to the view to show a hotzone.
            /// </summary>
            /// <param name="hotZoneId"></param>
            void ShowHotZone(HotZoneNode hotZone);

            /// <summary>
            /// Delegates to the view to remove a hotzone from the map.
            /// </summary>
            /// <param name="hotZoneId"></param>
            void HideHotZone(HotZoneNode hotZone);

            /// <summary>
            /// Returns the zoom level that the map is currently in.
            /// </summary>
            /// <returns></returns>
            int GetZoomLevel();
        }

        private IView _view;
        private GameContext _gameContext;
        public MapPresenter(IView view, GameContext gameContext)
        {
            _view = view;
            _gameContext = gameContext;
        }

        public void ZoomLevelChanged(int zoomLevel)
        {
            _gameContext.SetZoomLevel(zoomLevel);
        }
    }
}
