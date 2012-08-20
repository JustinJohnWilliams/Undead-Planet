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
using UndeadEarth.Model.Proxy;
using System.Windows.Media.Imaging;

namespace UndeadEarth.Silverlight
{
    public partial class HotZoneUserControl : UserControl
    {
        public interface ICommunicator
        {
            /// <summary>
            /// Delegates to the parent that this control needs to be brought to the front.
            /// </summary>
            /// <param name="hotZoneUserControl"></param>
            void BringToFront(HotZoneUserControl hotZoneUserControl);

            /// <summary>
            /// Delegates to the parent that this control needs to be sent to the back.
            /// </summary>
            /// <param name="hotZoneUserControl"></param>
            void SendToBack(HotZoneUserControl hotZoneUserControl);

            /// <summary>
            /// Delegates to the parent that a request was made to move to this hotzone.
            /// </summary>
            /// <param name="hotZoneUserControl"></param>
            void MoveRequested(HotZoneUserControl hotZoneUserControl);
        }

        private Node _hotZone;
        private string _baseUri;
        private static BitmapImage _hotZoneIcon;
        private static BitmapImage _clearedHotZoneIcon;
        private ICommunicator _communicator;
        public HotZoneUserControl(string baseUri, HotZoneNode node, ICommunicator communicator)
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(HotZoneUserControl_Loaded);

            _hotZone = node;
            _baseUri = baseUri;
            _communicator = communicator;

            if (_hotZoneIcon == null)
            {
                string uri = String.Concat(_baseUri, "Content/", "images/", "hotzone.png");
                _hotZoneIcon = new BitmapImage(new Uri(uri));
            }

            if (_clearedHotZoneIcon == null)
            {
                string uri = String.Concat(_baseUri, "Content/", "images/", "information.png");
                _clearedHotZoneIcon = new BitmapImage(new Uri(uri));
            }
        }

        /// <summary>
        /// Set control specific information on load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HotZoneUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            imageHotZone.Source = _hotZoneIcon;
            imageClearedHotZone.Source = _clearedHotZoneIcon;
        }

        /// <summary>
        /// Show details on mouse enter (on the image).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageHotZone_MouseEnter(object sender, MouseEventArgs e)
        {
            _communicator.BringToFront(this);
            MouseLeave += new MouseEventHandler(HotZoneUserControl_MouseLeave); 
        }

        /// <summary>
        /// Hide details on mouse leave (on the image).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HotZoneUserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            HideDetails();
        }

        public Node HotZone
        {
            get
            {
                return _hotZone;
            }
        }

        public void HideDetails()
        {
            MouseLeave -= new MouseEventHandler(HotZoneUserControl_MouseLeave); //Consider using Behavior<T>, TriggerAction<T>, TargetedTriggerAction<T>
            _communicator.SendToBack(this);
        }

        /// <summary>
        /// Requests the parent that a move was requested to the location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMove_Click(object sender, RoutedEventArgs e)
        {
            _communicator.MoveRequested(this);
        }

        private void imageHotZone_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _communicator.MoveRequested(this);
        }

        internal void UpdateHotZoneClearedStatus(bool isCleared)
        {
            if (isCleared)
            {
                imageHotZone.Visibility = System.Windows.Visibility.Collapsed;
                imageClearedHotZone.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                imageHotZone.Visibility = System.Windows.Visibility.Visible;
                imageClearedHotZone.Visibility =  System.Windows.Visibility.Collapsed;
            }
        }
    }
}
