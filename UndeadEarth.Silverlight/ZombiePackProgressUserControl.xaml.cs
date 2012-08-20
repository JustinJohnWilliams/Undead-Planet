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
using System.Windows.Media.Imaging;
using UndeadEarth.Silverlight.Model;

namespace UndeadEarth.Silverlight
{
    public partial class ZombiePackProgressUserControl : UserControl, HuntPresenter.IView
    {
        private static BitmapImage _huntIcon;

        private GameContext _gameContext;
        private HuntPresenter _presenter;
        private Storyboard _fadeIn;
        private Storyboard _fadeOut;
        public ZombiePackProgressUserControl()
        {
            InitializeComponent();

            if(App.IsNotInDesignMode(this))
            {
                _gameContext = App.GameContext;
                _presenter = new HuntPresenter(App.MapPresenterView, this, _gameContext, App.NotificationView);

                if (_huntIcon == null)
                {
                    string uri = String.Concat(_gameContext.BaseUri, "Content/", "images/", "zombie.png");
                    _huntIcon = new BitmapImage(new Uri(uri));
                    _fadeIn = App.AnimationProvider.GetFadeInAnimation(this);
                    _fadeOut = App.AnimationProvider.GetFadeOutAnimation(this);
                    _fadeOut.Completed += new EventHandler(_fadeOut_Completed);
                }

                Loaded += new RoutedEventHandler(ZombiePackProgressUserControl_Loaded);
            }
        }

        void _fadeOut_Completed(object sender, EventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        void ZombiePackProgressUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            //imageHunt.Source = _huntIcon;
            _presenter.Load();
        }

        void HuntPresenter.IView.NotifyCanHunt()
        {
            Visibility = System.Windows.Visibility.Visible;
            _fadeIn.Begin();
            buttonHunt.IsEnabled = true;
        }

        void HuntPresenter.IView.NotifyCannotHunt()
        {
            _fadeOut.Begin();
            buttonHunt.IsEnabled = false;
        }

        private void buttonHunt_Click(object sender, RoutedEventArgs e)
        {
            _presenter.Hunt();
        }

        void HuntPresenter.IView.PopulateDestructionProgress(int zombiesLeft, int maxZombies, int costForHunt)
        {
            textBlockPercent.Text = zombiesLeft.ToString() + " left out of " + maxZombies.ToString();
            progressBarDestructionProgress.Minimum = 0;
            progressBarDestructionProgress.Maximum = maxZombies;
            progressBarDestructionProgress.Value = zombiesLeft;
            textBlockEnergy.Text = costForHunt.ToString();
        }

        void HuntPresenter.IView.PopulateUserAttackPower(int attackPower)
        {
            textBlockAttackPower.Text = attackPower.ToString();
        }
    }
}
