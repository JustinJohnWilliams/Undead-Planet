using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl;
using UndeadEarth.Silverlight.Presenters;
using UndeadEarth.Model.Proxy;
using UndeadEarth.Silverlight.Proxy;
using System.Windows.Media.Imaging;
using UndeadEarth.Silverlight.Model;

namespace UndeadEarth.Silverlight
{
    public partial class MainPage :
        UserControl,
        UserPresenter.IView,
        HotZoneUserControl.ICommunicator,
        UserNodeUserControl.ICommunicator,
        ZombiePackUserControl.ICommunicator,
        StoreUserControl.ICommunicator,
        SafeHouseUserControl.ICommunicator,
        MapPresenter.IView,
        INotificationView
    {
        private UserPresenter _presenter;
        private MapPresenter _mapPresenter;
        private string _baseUri;
        UserNodeUserControl _userNodeUserControl;
        private DelayExecution _delayExecution;
        private Dictionary<Guid, ZombiePackUserControl> _zomebiePackUserControls;
        private Dictionary<Guid, HotZoneUserControl> _hotZoneUserControls;
        private Dictionary<Guid, StoreUserControl> _storeUserControls;
        private Dictionary<Guid, SafeHouseUserControl> _safeHouseUserControls;
        private static BitmapImage _moveIcon;

        public MainPage()
        {
            App.MapPresenterView = this;
            App.NotificationView = this;
            InitializeComponent();

            if (App.IsNotInDesignMode(this))
            {
                _baseUri = App.GameContext.BaseUri;

                //initialize the presenter with the base uri and the user id.
                _presenter = new UserPresenter(this, this, App.GameContext, App.NotificationView);
                _mapPresenter = new MapPresenter(this, App.GameContext);

                //delay execution set up for loading the presenter
                _delayExecution = new DelayExecution();

                //raise event when the bing map is loaded.
                mapUndeadEarth.Loaded += new RoutedEventHandler(mapUndeadEarth_Loaded);
                mapUndeadEarth.ViewChangeEnd += new EventHandler<MapEventArgs>(mapUndeadEarth_ViewChangeEnd);

                _zomebiePackUserControls = new Dictionary<Guid, ZombiePackUserControl>();

                _hotZoneUserControls = new Dictionary<Guid, HotZoneUserControl>();

                _storeUserControls = new Dictionary<Guid, StoreUserControl>();

                _safeHouseUserControls = new Dictionary<Guid, SafeHouseUserControl>();

                if (_moveIcon == null)
                {
                    string uri = String.Concat(_baseUri, "Content/", "images/", "move.png");
                    _moveIcon = new BitmapImage(new Uri(uri));
                }

                achievementUserControl.Visibility = Visibility.Collapsed;

                Application.Current.Host.Content.FullScreenChanged += new EventHandler(Content_FullScreenChanged);


                mapUndeadEarth.Width = 675;
                mapUndeadEarth.Height = 350;
                gridLocationDetails.Width = 675;
            }
        }

        void Content_FullScreenChanged(object sender, EventArgs e)
        {
            bool isFullScreen = Application.Current.Host.Content.IsFullScreen;
            if(isFullScreen)
            {
                mapUndeadEarth.Width = 1000;
                mapUndeadEarth.Height = 450;
                gridLocationDetails.Width = 1000;
                //gridLocationDetails.Width = 400;
                //gridLocationDetails.SetValue(Grid.ColumnProperty, 1);
                //gridLocationDetails.SetValue(Grid.RowProperty, 0);
            }
            else
            {
                mapUndeadEarth.Width = 675;
                mapUndeadEarth.Height = 350;
                gridLocationDetails.Width = 675;
                //gridLocationDetails.SetValue(Grid.ColumnProperty, 0);
                //gridLocationDetails.SetValue(Grid.RowProperty, 2);
                //gridLocationDetails.Width = 675;
            }
        }

        void mapUndeadEarth_ViewChangeEnd(object sender, MapEventArgs e)
        {
            _mapPresenter.ZoomLevelChanged(Convert.ToInt32(mapUndeadEarth.ZoomLevel));
        }

        void mapUndeadEarth_Loaded(object sender, RoutedEventArgs e)
        {
            //when the bing map is loaded, wait 3 seconds (for everything to render) and then call _persenter.Load();
            _delayExecution.SetTimeout(3000, () => _presenter.Load());
        }

        void MapPresenter.IView.InitializeUser(UserNode userNode)
        {
            textBlockUserName.Text = userNode.Name;
            //create a user node and add it to the bing map with the highest zindex 
            _userNodeUserControl = new UserNodeUserControl(_baseUri, userNode, this);
            _userNodeUserControl.SetValue(MapLayer.PositionProperty,
                new Location(Convert.ToDouble(userNode.Latitude),
                             Convert.ToDouble(userNode.Longitude)));

            _userNodeUserControl.SetValue(Canvas.ZIndexProperty, 10);

            mapUndeadEarth.Children.Add(_userNodeUserControl);
        }

        void MapPresenter.IView.MoveToUser(int zoomLevel)
        {
            //move the map to the current user's location
            Location userLocation = new Location(Convert.ToDouble(_userNodeUserControl.UserNode.Latitude),
                                    Convert.ToDouble(_userNodeUserControl.UserNode.Longitude));
            mapUndeadEarth.SetView(userLocation, zoomLevel);
        }

        void HotZoneUserControl.ICommunicator.BringToFront(HotZoneUserControl hotZoneUserControl)
        {
            SetEnergyToolTip(hotZoneUserControl.HotZone.Latitude, hotZoneUserControl.HotZone.Longitude);
            hotZoneUserControl.SetValue(Canvas.ZIndexProperty, 11);
        }

        void HotZoneUserControl.ICommunicator.SendToBack(HotZoneUserControl hotZoneUserControl)
        {
            borderStatus.Visibility = System.Windows.Visibility.Collapsed;
            hotZoneUserControl.SetValue(Canvas.ZIndexProperty, 5);
        }

        void MapPresenter.IView.UpdateUserLocation(double latitude, double longitude)
        {
            _userNodeUserControl.MoveUser(latitude, longitude);
            _userNodeUserControl.HideMessage();
            Cursor = Cursors.Arrow;
        }

        void HotZoneUserControl.ICommunicator.MoveRequested(HotZoneUserControl hotZoneUserControl)
        {
            hotZoneUserControl.HideDetails();
            Cursor = Cursors.Arrow;
            _presenter.Move(hotZoneUserControl.HotZone.Latitude, hotZoneUserControl.HotZone.Longitude);
        }


        void MapPresenter.IView.AddZombiePacks(List<Node> zombiePacks)
        {
            //for each hotzone, add it to the map with a z index lower than the user's
            foreach (Node node in zombiePacks)
            {
                if (!_zomebiePackUserControls.ContainsKey(node.Id))
                {
                    _zomebiePackUserControls.Add(node.Id, new ZombiePackUserControl(_baseUri, node, this));
                }

                ZombiePackUserControl userControl = _zomebiePackUserControls[node.Id];

                userControl.SetValue(MapLayer.PositionProperty,
                    new Location(Convert.ToDouble(node.Latitude),
                                 Convert.ToDouble(node.Longitude)));

                userControl.SetValue(Canvas.ZIndexProperty, 1);

                if (mapUndeadEarth.Children.Contains(userControl) == false)
                {
                    mapUndeadEarth.Children.Add(userControl);
                }
            }
        }

        void ZombiePackUserControl.ICommunicator.BringToFront(ZombiePackUserControl zombiePackUserControl)
        {
            SetEnergyToolTip(zombiePackUserControl.ZombieNode.Latitude, zombiePackUserControl.ZombieNode.Longitude);
            zombiePackUserControl.SetValue(Canvas.ZIndexProperty, 11);
        }

        void ZombiePackUserControl.ICommunicator.SendToBack(ZombiePackUserControl zombiePackUserControl)
        {
            borderStatus.Visibility = System.Windows.Visibility.Collapsed;
            zombiePackUserControl.SetValue(Canvas.ZIndexProperty, 1);
        }

        void ZombiePackUserControl.ICommunicator.MoveRequested(ZombiePackUserControl zombiePackUserControl)
        {
            zombiePackUserControl.HideDetails();
            Cursor = Cursors.Arrow;
            _presenter.Move(zombiePackUserControl.ZombieNode.Latitude, zombiePackUserControl.ZombieNode.Longitude);
        }

        void MapPresenter.IView.RemoveZombiePacks(List<Guid> zombiePacks)
        {
            //for each hotzone, add it to the map with a z index lower than the user's
            foreach (Guid nodeId in zombiePacks)
            {
                if (_zomebiePackUserControls.ContainsKey(nodeId))
                {
                    ZombiePackUserControl userControl = _zomebiePackUserControls[nodeId];
                    mapUndeadEarth.Children.Remove(userControl);
                }
            }
        }

        void MapPresenter.IView.UpdateHotZoneClearedStatus(Guid hotZoneId, bool isCleared)
        {
            if (_hotZoneUserControls.ContainsKey(hotZoneId))
            {
                _hotZoneUserControls[hotZoneId].UpdateHotZoneClearedStatus(isCleared);
            }
        }

        void UserPresenter.IView.PopulateEnergy(int currentEnergy, int totalEnergy)
        {
            textBlockCurrent.Text = currentEnergy.ToString();
            textBlockMax.Text = totalEnergy.ToString();
            progressBarEnergy.Maximum = totalEnergy;
            progressBarEnergy.Value = currentEnergy;
        }

        void UserPresenter.IView.PopulateUserLevel(int currentLevel, long currentKills, long neededKills, long lastLevelKills)
        {
            textBlockUserLevel.Text = currentLevel.ToString();

            if(currentKills == 0)
            {
                currentKills = 1;
            }

            progressBarUserLevel.Minimum = 0;
            progressBarUserLevel.Maximum = neededKills - lastLevelKills;
            progressBarUserLevel.Value = currentKills - lastLevelKills;
        }

        private MapPolygon _moveRadius;
        void MapPresenter.IView.UpdateUserMoveRadius(List<Location> locations)
        {
            if (_moveRadius == null)
            {
                _moveRadius = new MapPolygon { Fill = new SolidColorBrush(Colors.Transparent), Stroke = new SolidColorBrush(Colors.Green), StrokeThickness = 3, Opacity = .5 };
                mapUndeadEarth.Children.Add(_moveRadius);
            }

            _moveRadius.Locations = new LocationCollection();
            foreach (Location location in locations)
            {
                _moveRadius.Locations.Add(location);
            }
        }

        void UserPresenter.IView.PopulateMoney(int money)
        {
            textBlockMoney.Text = money.ToString("c");
        }

        void MapPresenter.IView.AddStores(List<StoreNode> stores)
        {
            foreach (StoreNode store in stores)
            {
                if (!_storeUserControls.ContainsKey(store.Id))
                {
                    _storeUserControls.Add(store.Id, new StoreUserControl(_baseUri, store, this));
                }

                StoreUserControl userControl = _storeUserControls[store.Id];

                userControl.SetValue(MapLayer.PositionProperty,
                    new Location(Convert.ToDouble(store.Latitude),
                                Convert.ToDouble(store.Longitude)));

                userControl.SetValue(Canvas.ZIndexProperty, 1);

                if (mapUndeadEarth.Children.Contains(userControl) == false)
                {
                    mapUndeadEarth.Children.Add(userControl);
                }
            }
        }

        void MapPresenter.IView.RemoveStores(List<StoreNode> stores)
        {
            foreach (StoreNode store in stores)
            {
                if (_storeUserControls.ContainsKey(store.Id))
                {
                    StoreUserControl userControl = _storeUserControls[store.Id];
                    mapUndeadEarth.Children.Remove(userControl);
                }
            }
        }

        void StoreUserControl.ICommunicator.BringToFront(StoreUserControl storeUserControl)
        {
            SetEnergyToolTip(storeUserControl.Store.Latitude, storeUserControl.Store.Longitude);
            storeUserControl.SetValue(Canvas.ZIndexProperty, 11);
        }

        void StoreUserControl.ICommunicator.SendToBack(StoreUserControl storeUserControl)
        {
            borderStatus.Visibility = System.Windows.Visibility.Collapsed;
            storeUserControl.SetValue(Canvas.ZIndexProperty, 1);
        }

        void StoreUserControl.ICommunicator.MoveRequested(StoreUserControl storeUserControl)
        {
            storeUserControl.HideDetails();
            Cursor = Cursors.Arrow;
            _presenter.Move(storeUserControl.Store.Latitude, storeUserControl.Store.Longitude);
        }

        void MapPresenter.IView.ShowHotZone(HotZoneNode hotZoneNode)
        {
            AddHotZoneUserControlIfNecessary(hotZoneNode);
            if (_hotZoneUserControls.ContainsKey(hotZoneNode.Id))
            {
                HotZoneUserControl control = _hotZoneUserControls[hotZoneNode.Id];
                if (mapUndeadEarth.Children.Contains(control) == false)
                {
                    mapUndeadEarth.Children.Add(control);
                }
            }
        }

        void MapPresenter.IView.HideHotZone(HotZoneNode hotZoneNode)
        {
            AddHotZoneUserControlIfNecessary(hotZoneNode);
            if (_hotZoneUserControls.ContainsKey(hotZoneNode.Id))
            {
                HotZoneUserControl control = _hotZoneUserControls[hotZoneNode.Id];
                if (mapUndeadEarth.Children.Contains(control) == true)
                {
                    mapUndeadEarth.Children.Remove(control);
                }
            }
        }

        private void AddHotZoneUserControlIfNecessary(HotZoneNode hotZoneNode)
        {
            if (_hotZoneUserControls.ContainsKey(hotZoneNode.Id) == false)
            {
                HotZoneUserControl userControl = new HotZoneUserControl(_baseUri, hotZoneNode, this);
                _hotZoneUserControls.Add(hotZoneNode.Id, userControl);

                userControl.SetValue(MapLayer.PositionProperty,
                    new Location(Convert.ToDouble(hotZoneNode.Latitude),
                                 Convert.ToDouble(hotZoneNode.Longitude)));

                userControl.SetValue(Canvas.ZIndexProperty, 5);
            }
        }

        //collapses inventory information
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridUserItems.Visibility == Visibility.Collapsed)
            {
                dataGridUserItems.Visibility = Visibility.Visible;
            }
            else
            {
                dataGridUserItems.Visibility = Visibility.Collapsed;
            }
        }

        //collapses user information
        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (borderUserInformation.Visibility == Visibility.Collapsed)
            {
                borderUserInformation.Visibility = Visibility.Visible;
            }
            else
            {
                borderUserInformation.Visibility = Visibility.Collapsed;
            }
        }

        void UserPresenter.IView.PopulateInventory(List<KeyValuePair<Item, string>> items)
        {
            dataGridUserItems.ItemsSource = items;
        }

        void UserPresenter.IView.PopulateAttackPower(int attackPower)
        {
            textBlockPower.Text = attackPower.ToString();
        }

        void SafeHouseUserControl.ICommunicator.BringToFront(SafeHouseUserControl safeHouseUserControl)
        {
            SetEnergyToolTip(safeHouseUserControl.SafeHouse.Longitude, safeHouseUserControl.SafeHouse.Longitude);
            safeHouseUserControl.SetValue(Canvas.ZIndexProperty, 11);
        }

        void SafeHouseUserControl.ICommunicator.SendToBack(SafeHouseUserControl safeHouseUserControl)
        {
            borderStatus.Visibility = System.Windows.Visibility.Collapsed;
            safeHouseUserControl.SetValue(Canvas.ZIndexProperty, 1);
        }

        void SafeHouseUserControl.ICommunicator.MoveRequested(SafeHouseUserControl safeHouseUserControl)
        {
            safeHouseUserControl.HideDetails();
            Cursor = Cursors.Arrow;
            _presenter.Move(safeHouseUserControl.SafeHouse.Latitude, safeHouseUserControl.SafeHouse.Longitude);
        }

        void MapPresenter.IView.AddSafeHouses(List<SafeHouseNode> safeHouses)
        {
            foreach (SafeHouseNode safeHouse in safeHouses)
            {
                if (!_safeHouseUserControls.ContainsKey(safeHouse.Id))
                {
                    _safeHouseUserControls.Add(safeHouse.Id, new SafeHouseUserControl(_baseUri, safeHouse, this));
                }

                SafeHouseUserControl userControl = _safeHouseUserControls[safeHouse.Id];

                userControl.SetValue(MapLayer.PositionProperty,
                    new Location(Convert.ToDouble(safeHouse.Latitude),
                                Convert.ToDouble(safeHouse.Longitude)));

                userControl.SetValue(Canvas.ZIndexProperty, 1);

                if (mapUndeadEarth.Children.Contains(userControl) == false)
                {
                    mapUndeadEarth.Children.Add(userControl);
                }
            }
        }

        void MapPresenter.IView.RemoveSafeHouses(List<SafeHouseNode> safeHouses)
        {
            // removes safe houses outside of viewing radius
            foreach (SafeHouseNode safeHouse in safeHouses)
            {
                if (_safeHouseUserControls.ContainsKey(safeHouse.Id))
                {
                    SafeHouseUserControl userControl = _safeHouseUserControls[safeHouse.Id];
                    mapUndeadEarth.Children.Remove(userControl);
                }
            }
        }

        void INotificationView.Notify(string message)
        {
            popupUserControl.Show(message);
        }

        private void buttonUse_Click(object sender, RoutedEventArgs e)
        {
            _presenter.UseItem((e.OriginalSource as Button).DataContext as Item);
        }

        void UserPresenter.IView.PopulateAchievements(List<Achievement> achievements)
        {
            achievementUserControl.PopulateAchievements(achievements);
        }

        void UserPresenter.IView.NotifyNewAchievement(List<string> achievements)
        {
            achievementUserControl.Visibility = System.Windows.Visibility.Visible;
            achievementUserControl.NewAchievements(achievements);
        }

        private void buttonFullScreen_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
        }

        private void buttonZoomToUser_Click(object sender, RoutedEventArgs e)
        {
            _presenter.RequestGoToUser();
        }

        private void buttonZoomIn_Click(object sender, RoutedEventArgs e)
        {
            mapUndeadEarth.ZoomLevel = mapUndeadEarth.ZoomLevel + 1;
        }

        private void buttonZoomOut_Click(object sender, RoutedEventArgs e)
        {
            mapUndeadEarth.ZoomLevel = mapUndeadEarth.ZoomLevel - 1;
        }

        private void buttonInviteFriend_Click(object sender, RoutedEventArgs e)
        {

        }

        int MapPresenter.IView.GetZoomLevel()
        {
            return Convert.ToInt32(mapUndeadEarth.ZoomLevel);
        }

        void UserPresenter.IView.ShowTutorial()
        {
            tutorialUserControl.Visibility = Visibility.Visible;
        }

        void UserPresenter.IView.NotifyLevelUp()
        {
            levelUpUserControl.ShowLevelUp();
        }

        public void SetEnergyToolTip(double latitude, double longitude)
        {
            borderStatus.Visibility = System.Windows.Visibility.Visible;
            textBlockStatus.Text = "Click to move here.  Energy cost: " + _presenter.GetDistanceFromUser(latitude, longitude).ToString();
        }

        void UserPresenter.IView.NotifyMoreEnergyCountDown(int energyAmount, int secondsUntil)
        {
            textBlockEnergyCountDown.Text = energyAmount + " energy in " + secondsUntil + " seconds...";
        }
    }
}
