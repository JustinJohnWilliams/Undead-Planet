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
    public partial class SafeHouseUserControl : UserControl
    {
        public interface ICommunicator
        {
            /// <summary>
            /// Delegates to the parent that this control needs to be brought to the front.
            /// </summary>
            /// <param name="safeHouseUserControl"></param>
            void BringToFront(SafeHouseUserControl safeHouseUserControl);

            /// <summary>
            /// Delegates to the parent that this control needs to be sent to the back.
            /// </summary>
            /// <param name="safeHouseUserControl"></param>
            void SendToBack(SafeHouseUserControl safeHouseUserControl);

            /// <summary>
            /// Delegates to the parent that a request was made to move to this hotzone.
            /// </summary>
            /// <param name="safeHouseUserControl"></param>
            void MoveRequested(SafeHouseUserControl safeHouseUserControl);
        }

        private SafeHouseNode _safeHouseNode;
        private string _baseUri;
        private static BitmapImage _safeHouseIcon;
        private ICommunicator _communicator;

        public SafeHouseNode SafeHouse
        {
            get
            {
                return _safeHouseNode;
            }
        }

        public SafeHouseUserControl(string baseUri, SafeHouseNode node, ICommunicator communicator)
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(SafeHouseUserControl_Loaded);

            _safeHouseNode = node;
            _baseUri = baseUri;
            _communicator = communicator;

            if (_safeHouseIcon == null)
            {
                string uri = String.Concat(_baseUri, "Content/", "images/", "safehouse.png");
                _safeHouseIcon = new BitmapImage(new Uri(uri));
            }
        }

        /// <summary>
        /// Set the control specific on load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SafeHouseUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            imageSafeHouse.Source = _safeHouseIcon;
        }

        /// <summary>
        /// Show details on Mouse Enter (on the image)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageSafeHouse_MouseEnter(object sender, MouseEventArgs e)
        {
            _communicator.BringToFront(this);
            MouseLeave += new MouseEventHandler(SafeHouseUserControl_MouseLeave);
        }

        void SafeHouseUserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            HideDetails();
        }

        public void HideDetails()
        {
            MouseLeave -= new MouseEventHandler(SafeHouseUserControl_MouseLeave);
            _communicator.SendToBack(this);
        }

        private void imageSafeHouse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            _communicator.MoveRequested(this);
        }
    }
}
