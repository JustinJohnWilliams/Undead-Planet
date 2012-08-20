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
    public partial class ShopDetailUserControl : UserControl, StorePresenter.IView
    {
        private StorePresenter _presenter;

        public ShopDetailUserControl()
        {
            InitializeComponent();

            if (App.IsNotInDesignMode(this))
            {
                _presenter = new StorePresenter(App.MapPresenterView, this, App.GameContext, App.NotificationView);

                Loaded += new RoutedEventHandler(ShopDetailUserControl_Loaded);
            }
        }

        void ShopDetailUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            _presenter.Load();
        }

        void StorePresenter.IView.NotifyCanShop()
        {
            this.Visibility = Visibility.Visible;
        }

        void StorePresenter.IView.NotifyCannotShop()
        {
            this.Visibility = Visibility.Collapsed;
        }

        void StorePresenter.IView.PopulateStoreInventory(List<Item> items)
        {
            dataGridTransactions.DataContext = items;
        }

        void StorePresenter.IView.PopulateUserMoney(int money)
        {
            txtBlockUserMoney.Text = money.ToString("c");
        }

        private void buttonBuy_Click(object sender, RoutedEventArgs e)
        {
            _presenter.Buy((e.OriginalSource as Button).DataContext as Item);
        }

        void StorePresenter.IView.PopulateUserInventory(List<Item> items)
        {
            dataGridUserInventory.DataContext = items;
        }
    }
}
