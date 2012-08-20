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
using System.Windows.Media.Imaging;
using UndeadEarth.Model.Proxy;
using UndeadEarth.Silverlight.Proxy;
using Microsoft.Maps.MapControl;

namespace UndeadEarth.Silverlight
{
    public partial class UserNodeUserControl : UserControl
    {
        public interface ICommunicator
        {
        }

        private string _baseUri;
        private UserNode _userNode;
        private static BitmapImage _moveIcon;
        private ICommunicator _communicator;

        public UserNodeUserControl(string baseUri, UserNode userNode, ICommunicator communicator)
        {
            InitializeComponent();

            _baseUri = baseUri;
            _userNode = userNode;
            _communicator = communicator;

            Loaded += new RoutedEventHandler(UserNodeUserControl_Loaded);

            if (_moveIcon == null)
            {
                string uri = String.Concat(_baseUri, "Content/", "images/", "move.png");
                _moveIcon = new BitmapImage(new Uri(uri));
            }
        }

        void UserNodeUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string uri = String.Concat(_baseUri, "Content/", "images/", "you.png");
            imageUser.Source = new BitmapImage(new Uri(uri));
        }

        /// <summary>
        /// Gets the UserNode that this node represents.
        /// </summary>
        public UserNode UserNode
        {
            get
            {
                return _userNode;
            }
        }

        /// Integrates the movement animation and location update for user.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public void MoveUser(double latitude, double longitude)
        {
            //set the latitude and longitude
            _userNode.Latitude = latitude;
            _userNode.Longitude = longitude;

            EventHandler onComplete = null;

            //action to execute when the hide animation for the user icon completes.
            onComplete = (sender, e) =>
            {
                //set the usercontrol on the map.
                SetValue(MapLayer.PositionProperty,
                            new Location(Convert.ToDouble(latitude),
                                         Convert.ToDouble(longitude)));

                //show the icon again
                storyBoardUserIconShow.Begin();

                //detach completed event
                storyBoardUserIconHide.Completed -= onComplete;
            };

            storyBoardUserIconHide.Completed += onComplete;
            storyBoardUserIconHide.Begin();
        }

        public void ShowMessage(string message)
        {
            textblockMessage.Visibility = System.Windows.Visibility.Visible;
            borderMessage.Visibility = System.Windows.Visibility.Visible;
            textblockMessage.Text = message;
        }

        public void HideMessage()
        {
            textblockMessage.Visibility = System.Windows.Visibility.Collapsed;
            borderMessage.Visibility = System.Windows.Visibility.Collapsed;
            textblockMessage.Text = "";
        }
    }
}
