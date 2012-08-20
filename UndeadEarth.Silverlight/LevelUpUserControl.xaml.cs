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
using UndeadEarth.Silverlight.Presenters;

namespace UndeadEarth.Silverlight
{
    public partial class LevelUpUserControl : UserControl, LevelUpPresenter.IView
    {
        private static BitmapImage _greenArrow;
        private LevelUpPresenter _levelUpPresenter;

        public LevelUpUserControl()
        {
            InitializeComponent();

            if(App.IsNotInDesignMode(this))
            {
                if (_greenArrow == null)
                {
                    string uri = String.Concat(App.GameContext.BaseUri, "Content/", "images/", "greenarrow.png");
                    _greenArrow = new BitmapImage(new Uri(uri));
                }

                if (App.IsNotInDesignMode(this))
                {
                    _levelUpPresenter = new LevelUpPresenter(this, App.GameContext);
                }

                Loaded += new RoutedEventHandler(LevelUpUserControl_Loaded);
            }
        }

        void LevelUpUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            imageArrowAttack.Source = _greenArrow;
            imageArrowEnergy.Source = _greenArrow;
            imageArrowItem.Source = _greenArrow;
            imageArrowLevel.Source = _greenArrow;
        }

        public void ShowLevelUp()
        {
            Visibility = Visibility.Visible;
            _levelUpPresenter.RequestLevelInfo();
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        void LevelUpPresenter.IView.PopulateNewLevel(int newLevel)
        {
            textBlockCurrentLevel.Text = newLevel.ToString();
        }

        void LevelUpPresenter.IView.PopulateNewAttack(int newAttack)
        {
            textBlockCurrentAttack.Text = newAttack.ToString();
        }

        void LevelUpPresenter.IView.PopulateNewEnergy(int newEnergy)
        {
            textBlockCurrentEnergy.Text = newEnergy.ToString();
        }

        void LevelUpPresenter.IView.PopulateNewZombie(long zombiesNeededForNextLevel)
        {
            textBlockZombiesToNextLevel.Text = zombiesNeededForNextLevel.ToString();
        }

        void LevelUpPresenter.IView.PopulateNewItemSlot(int newItemSlot)
        {
            textBlockCurrentItem.Text = newItemSlot.ToString();
        }
    }
}
