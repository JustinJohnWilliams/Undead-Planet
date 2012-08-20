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

namespace UndeadEarth.Silverlight
{
    public partial class AchievementUserControl : UserControl
    {
        public AchievementUserControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        public void PopulateAchievements(List<Achievement> achievements)
        {
            itemsControlAchievement.DataContext = achievements;
        }

        public void NewAchievements(List<string> newAchievements)
        {
            itemsControlNewAchievements.DataContext = newAchievements;
        }
    }
}
