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
    public partial class StoreUserControl : UserControl
    {
        public interface ICommunicator
        {
            /// <summary>
            /// Delegates to the parent that this control needs to be brought to the front.
            /// </summary>
            /// <param name="storeUserControl"></param>
            void BringToFront(StoreUserControl storeUserControl);

            /// <summary>
            /// Delegates to the parent that this control needs to be sent to the back.
            /// </summary>
            /// <param name="storeUserControl"></param>
            void SendToBack(StoreUserControl storeUserControl);

            /// <summary>
            /// Delegates to the parent that a request was made to move to this hotzone.
            /// </summary>
            /// <param name="storeUserControl"></param>
            void MoveRequested(StoreUserControl storeUserControl);
        }

        private Node _store;
        private string _baseUri;
        private static BitmapImage _storeIcon;
        private ICommunicator _communicator;

        public Node Store
        {
            get
            {
                return _store;
            }
        }

        public void HideDetails()
        {
            MouseLeave -= new MouseEventHandler(StoreUserControl_MouseLeave);
            _communicator.SendToBack(this);
        }

        void StoreUserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            HideDetails();
        }

        public StoreUserControl(string baseUri, StoreNode node, ICommunicator communicator)
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(StoreUserControl_Loaded);

            _store = node;
            _baseUri = baseUri;
            _communicator = communicator;

            if (_storeIcon == null)
            {
                string uri = String.Concat(_baseUri, "Content/", "images/", "store.png");
                _storeIcon = new BitmapImage(new Uri(uri));
            }
        }

        /// <summary>
        /// Set the control specific information on load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StoreUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            imageStore.Source = _storeIcon;
        }

        /// <summary>
        /// Show details on Mouse Enter (on the image)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageStore_MouseEnter(object sender, MouseEventArgs e)
        {
            _communicator.BringToFront(this);
            MouseLeave += new MouseEventHandler(StoreUserControl_MouseLeave);
        }

        private void imageStore_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            _communicator.MoveRequested(this);
        }
    }
}
