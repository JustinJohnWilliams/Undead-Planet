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
using UndeadEarth.Silverlight.Model;
using RestfulSilverlight.Core;
using System.Linq;

namespace UndeadEarth.Silverlight.Presenters
{
    public class HotZonePresenter
    {
        public interface IView 
        {
            void PopulateHotZoneProgress(string hotZoneName, int currentZombiePackCount);
            void NotifyShowProgress();
            void NotifyHideProgress();
        }

        /// <summary>
        /// Reference to all hotzones on the map.
        /// </summary>
        private List<HotZoneNode> _hotZones;

        /// <summary>
        /// Reference to the hotzones and how many zombie packs are left.
        /// </summary>
        private List<KeyValuePair<Guid, int>> _zombiePackCounts;

        /// <summary>
        /// Represents the users current hotzone.
        /// </summary>
        private Guid _currentHotZoneId;


        private GameContext _gameContext;
        private MapPresenter.IView _mapPresenterView;
        private IView _view;
        public HotZonePresenter(MapPresenter.IView mapPresenterView, IView view, GameContext gameContext)
        {
            _gameContext = gameContext;
            _gameContext.UserMoved += new EventHandler(_gameContext_UserMoved);
            _gameContext.ZombiePackDestroyed += new EventHandler<ZombiePackDestroyedEventArgs>(_gameContext_ZombiePackDestroyed);
            _gameContext.ZoomLevelChanged += new EventHandler<ZoomLevelChangedEventArgs>(_gameContext_ZoomLevelChanged);
            _gameContext.UserInitalized += new EventHandler(_gameContext_UserInitalized);
            _mapPresenterView = mapPresenterView;
            _zombiePackCounts = new List<KeyValuePair<Guid, int>>();
            _currentHotZoneId = Guid.Empty;
            _view = view;
        }

        void _gameContext_UserInitalized(object sender, EventArgs e)
        {
            LoadHotZones();
        }

        bool _allHotZonesAreOnMap = false;
        void _gameContext_ZoomLevelChanged(object sender, ZoomLevelChangedEventArgs e)
        {
            if (_hotZones != null)
            {
                if (e.ZoomLevel < 8 && _allHotZonesAreOnMap == false)
                {
                    foreach (HotZoneNode node in _hotZones)
                    {
                        _mapPresenterView.ShowHotZone(node);
                        SetHotZoneClearedIfApplicable(node.Id);
                    }

                    _allHotZonesAreOnMap = true;
                }
                else if (e.ZoomLevel > 8 && _allHotZonesAreOnMap == true) 
                {
                    foreach (HotZoneNode node in _hotZones)
                    {
                        _mapPresenterView.HideHotZone(node);
                    }

                    UpdateZone(forceUpdate: true);
                    
                    _allHotZonesAreOnMap = false;
                }
            }
        }

        void _gameContext_ZombiePackDestroyed(object sender, ZombiePackDestroyedEventArgs e)
        {
            DetermineIfHotZoneCleared();
        }

        void _gameContext_UserMoved(object sender, EventArgs e)
        {
            if (_hotZones != null)
            {
                DetermineIfOnHotZone();
            }
        }

        private void LoadHotZones()
        {
            //initalize the hotzones if they have not been initialized yet
            if (_hotZones == null)
            {
                //on the retrieval of hozones, populate honzones on the map.
                AsyncDelegation ad2 = new AsyncDelegation();

                ad2.Get<List<HotZoneNode>>(new { controller = "HotZones", action = "Index", userId = _gameContext.UserId })
                    .WhenFinished(
                    (hotZones) =>
                    {
                        _hotZones = hotZones;
                    })
                    .ThenGet<List<KeyValuePair<Guid, int>>>(new { controller = "HotZones", action = "Uncleared", userId = _gameContext.UserId })
                    .WhenFinished(
                    (uncleared) =>
                    {
                        _zombiePackCounts = uncleared;
                        List<Guid> clearedHotZones =
                            _hotZones.Where(h => _zombiePackCounts.Any(z => z.Key == h.Id) == false)
                                     .Select(h => h.Id)
                                     .ToList();

                        foreach (Guid id in clearedHotZones)
                        {
                            _mapPresenterView.UpdateHotZoneClearedStatus(id, true);
                        }

                        UpdateZone(forceUpdate: false);
                        DetermineIfOnHotZone();
                    });

                ad2.Go();
            }
        }

        private void DetermineIfOnHotZone()
        {
            var hotZone = _hotZones.SingleOrDefault(s => s.Latitude == _gameContext.UserNode.Latitude && _gameContext.UserNode.Longitude == s.Longitude);
            if (hotZone != null)
            {
                int currentCount = 0;
                if(_zombiePackCounts.Any(z => z.Key == hotZone.Id))
                {
                    currentCount  = _zombiePackCounts.First(z => z.Key == hotZone.Id).Value;
                }

                _view.NotifyShowProgress();
                _view.PopulateHotZoneProgress(hotZone.Name, currentCount);
            }
            else
            {
                _view.NotifyHideProgress();
            }
        }

        private void UpdateZone(bool forceUpdate)
        {
            Guid previousHotZoneId = _currentHotZoneId;
            //show the hotzone the user is currently associated with on the map
            AsyncDelegation ad = new AsyncDelegation();

            //get teh current zone the user is in
            ad.Get<GuidResult>(new { controller = "Users", action = "Zone", userId = _gameContext.UserId })
                //for that zone, determine if there are any zombie packs left
              .WhenFinished(
              (result) =>
              {
                  _currentHotZoneId = result.Value;
                  ShowHideCurrentHotZone(previousHotZoneId, _currentHotZoneId, forceUpdate);
              });

            ad.Go();
        }

        private void DetermineIfHotZoneCleared()
        {
            Guid previousHotZoneId = _currentHotZoneId;
            AsyncDelegation ad = new AsyncDelegation();

            //get teh current zone the user is in
            ad.Get<GuidResult>(new { controller = "Users", action = "Zone", userId = _gameContext.UserId })
                //for that zone, determine if there are any zombie packs left
              .WhenFinished(
              (result) =>
              {
                  _currentHotZoneId = result.Value;
                  ShowHideCurrentHotZone(previousHotZoneId, _currentHotZoneId, forceUpdate: false);
              })
              .ThenGet<IntResult>().ForRoute(() => new { controller = "HotZones", action = "ZombiePacksLeft", userId = _gameContext.UserId, hotZoneId = _currentHotZoneId })
                //updae the ZombiePacks count to reflect the counts returned by Hotzones/ZombiePacksLeft
              .WhenFinished(
              (zombiePackCount) =>
              {
                  //update zombie pack counts
                  KeyValuePair<Guid, int>? kvp = _zombiePackCounts.SingleOrDefault(c => c.Key == _currentHotZoneId);
                  if (kvp != null)
                  {
                      _zombiePackCounts.Remove(kvp.Value);
                      _zombiePackCounts.Add(new KeyValuePair<Guid, int>(_currentHotZoneId, zombiePackCount.Value));
                  }

                  //if the zombie pack count is 0 then that means the hotzone is destroyed.  Notify game context that the hotzone has been destroyed.
                  if (zombiePackCount.Value == 0)
                  {
                      _mapPresenterView.UpdateHotZoneClearedStatus(_currentHotZoneId, true);
                  }
              });

            ad.Go();
        }

        public void Load()
        {
            _view.NotifyHideProgress();
        }

        private void ShowHideCurrentHotZone(Guid previousHotZoneId, Guid currentHotZoneId, bool forceUpdate)
        {
            HotZoneNode previousNode = _hotZones.SingleOrDefault(s => s.Id == previousHotZoneId);
            HotZoneNode currentNode = _hotZones.SingleOrDefault(s => s.Id == currentHotZoneId);
            if (currentHotZoneId != previousHotZoneId || forceUpdate == true)
            {
                if (previousNode != null)
                {
                    _mapPresenterView.HideHotZone(previousNode);
                }

                if (currentNode != null)
                {
                    _mapPresenterView.ShowHotZone(currentNode);
                    SetHotZoneClearedIfApplicable(currentHotZoneId);
                }
            }
        }

        private void SetHotZoneClearedIfApplicable(Guid hotZoneId)
        {
            if (_zombiePackCounts.Any(s => s.Key == hotZoneId))
            {
                int count = _zombiePackCounts.Single(s => s.Key == hotZoneId).Value;
                if (count == 0)
                {
                    _mapPresenterView.UpdateHotZoneClearedStatus(hotZoneId, true);
                }
            }
            else
            {
                _mapPresenterView.UpdateHotZoneClearedStatus(hotZoneId, true);
            }
        }
    }
}
