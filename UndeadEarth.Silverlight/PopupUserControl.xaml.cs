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

namespace UndeadEarth.Silverlight
{
    public partial class PopupUserControl : UserControl
    {
        public PopupUserControl()
        {
            InitializeComponent();
        }

        public void Show(string message)
        {
            Visibility = System.Windows.Visibility.Visible;
            textBlockName.Text = message;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
