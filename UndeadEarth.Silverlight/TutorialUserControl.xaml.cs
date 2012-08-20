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

namespace UndeadEarth.Silverlight
{
    public partial class TutorialUserControl : UserControl
    {

        private static BitmapImage _hotZoneIcon;
        private static BitmapImage _storeIcon;
        private static BitmapImage _zombiePackIcon;
        private static BitmapImage _safeHouseIcon;

        public TutorialUserControl()
        {
            InitializeComponent();
            if(App.IsNotInDesignMode(this))
            {
                if (_hotZoneIcon == null)
                {
                    string uri = String.Concat(App.GameContext.BaseUri, "Content/", "images/", "hotzone.png");
                    _hotZoneIcon = new BitmapImage(new Uri(uri));
                }

                if (_storeIcon == null)
                {
                    string uri = String.Concat(App.GameContext.BaseUri, "Content/", "images/", "store.png");
                    _storeIcon = new BitmapImage(new Uri(uri));
                }

                if (_zombiePackIcon == null)
                {
                    string uri = String.Concat(App.GameContext.BaseUri, "Content/", "images/", "zombie.png");
                    _zombiePackIcon = new BitmapImage(new Uri(uri));
                }

                if (_safeHouseIcon == null)
                {
                    string uri = String.Concat(App.GameContext.BaseUri, "Content/", "images/", "safehouse.png");
                    _safeHouseIcon = new BitmapImage(new Uri(uri));
                }

                Loaded += new RoutedEventHandler(TutorialUserControl_Loaded);
            }
        }

        void TutorialUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            imageHotzone.Source = _hotZoneIcon;
            imageSafeHouse.Source = _safeHouseIcon;
            imageShop.Source = _storeIcon;
            imageZombie.Source = _zombiePackIcon;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
