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
using UndeadEarth.Silverlight.Presenters;
using UndeadEarth.Model.Proxy;

namespace UndeadEarth.Silverlight
{
    public partial class SafeHouseDetailUserControl : UserControl, SafeHousePresenter.IView
    {
        private SafeHousePresenter _presenter;

        public SafeHouseDetailUserControl()
        {
            InitializeComponent();

            if (App.IsNotInDesignMode(this))
            {
                _presenter = new SafeHousePresenter(App.MapPresenterView, this, App.GameContext, App.NotificationView);

                Loaded += new RoutedEventHandler(SafeHouseDetailUserControl_Loaded);
            }
        }

        void SafeHouseDetailUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            _presenter.Load();
        }

        void SafeHousePresenter.IView.NotifyCanVisitSafeHouse()
        {
            this.Visibility = Visibility.Visible;
        }

        void SafeHousePresenter.IView.NotifyCannotVisitSafeHouse()
        {
            this.Visibility = Visibility.Collapsed;
        }

        void SafeHousePresenter.IView.PopulateSafeHouseInventory(List<Item> items)
        {
            dataGridSafeHouseItems.DataContext = items;
        }

        void SafeHousePresenter.IView.PopulateUserInventory(List<Item> items)
        {
            dataGridUserItems.DataContext = items;
        }

        private void btnToSafeHouse_Click(object sender, RoutedEventArgs e)
        {
            Guid itemId = ((e.OriginalSource as Button).DataContext as Item).Id;
            _presenter.TransferItemFromUserToSafeHouse(itemId);
        }

        private void btnToUser_Click(object sender, RoutedEventArgs e)
        {
            Guid itemId = ((e.OriginalSource as Button).DataContext as Item).Id;
            _presenter.TransferItemFromSafeHouseToUser(itemId);
        }
    }
}
