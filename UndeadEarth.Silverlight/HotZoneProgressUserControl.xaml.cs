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

namespace UndeadEarth.Silverlight
{
    public partial class HotZoneProgressUserControl : UserControl, HotZonePresenter.IView
    {
        private HotZonePresenter _hotZonePresenter;
        private Storyboard _fadeIn;
        private Storyboard _fadeOut;
        public HotZoneProgressUserControl()
        {
            InitializeComponent();
            if (App.IsNotInDesignMode(this))
            {
                _hotZonePresenter = new HotZonePresenter(App.MapPresenterView, this, App.GameContext);
                _fadeIn = App.AnimationProvider.GetFadeInAnimation(this);
                _fadeOut = App.AnimationProvider.GetFadeOutAnimation(this);
                _fadeOut.Completed += new EventHandler(_fadeOut_Completed);
                Loaded += new RoutedEventHandler(HotZoneProgressUserControl_Loaded);
            }
        }

        void _fadeOut_Completed(object sender, EventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        void HotZoneProgressUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Opacity = 0;
            _hotZonePresenter.Load();
        }

        #region IView Members

        void HotZonePresenter.IView.PopulateHotZoneProgress(string hotZoneName, int currentZombiePackCount)
        {
            textBlockHotZoneName.Text = hotZoneName;
            if (currentZombiePackCount == 0)
            {
                textBlockZombiePacksLeft.Text = "This Hot Zone is destroyed!";
            }
            else
            {
                textBlockZombiePacksLeft.Text = currentZombiePackCount.ToString();
            }
        }

        void HotZonePresenter.IView.NotifyShowProgress()
        {
            Visibility = System.Windows.Visibility.Visible;
            _fadeIn.Begin();
        }

        void HotZonePresenter.IView.NotifyHideProgress()
        {
            _fadeOut.Begin();
        }

        #endregion
    }
}
