﻿#pragma checksum "c:\Users\juwilliams\Desktop\Personal\Undead-Planet\UndeadEarth.Silverlight\UserNodeUserControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "11D93444F913F41003356D3DC8AA2F48"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace UndeadEarth.Silverlight {
    
    
    public partial class UserNodeUserControl : System.Windows.Controls.UserControl {
        
        internal System.Windows.Media.Animation.Storyboard storyBoardUserIconShow;
        
        internal System.Windows.Media.Animation.Storyboard storyBoardUserIconHide;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Image imageUser;
        
        internal System.Windows.Controls.Border borderMessage;
        
        internal System.Windows.Controls.TextBlock textblockMessage;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/UndeadEarth.Silverlight;component/UserNodeUserControl.xaml", System.UriKind.Relative));
            this.storyBoardUserIconShow = ((System.Windows.Media.Animation.Storyboard)(this.FindName("storyBoardUserIconShow")));
            this.storyBoardUserIconHide = ((System.Windows.Media.Animation.Storyboard)(this.FindName("storyBoardUserIconHide")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.imageUser = ((System.Windows.Controls.Image)(this.FindName("imageUser")));
            this.borderMessage = ((System.Windows.Controls.Border)(this.FindName("borderMessage")));
            this.textblockMessage = ((System.Windows.Controls.TextBlock)(this.FindName("textblockMessage")));
        }
    }
}

