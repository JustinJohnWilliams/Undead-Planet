﻿#pragma checksum "c:\Users\juwilliams\Desktop\Personal\Undead-Planet\UndeadEarth.Silverlight\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "8437605AE5692DDEDFCA4D41140A223A"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Maps.MapControl;
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
using UndeadEarth.Silverlight;


namespace UndeadEarth.Silverlight {
    
    
    public partial class MainPage : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.RowDefinition rowForMap;
        
        internal Microsoft.Maps.MapControl.Map mapUndeadEarth;
        
        internal System.Windows.Controls.Border borderStatus;
        
        internal System.Windows.Controls.TextBlock textBlockStatus;
        
        internal System.Windows.Controls.TextBlock textBlockUserName;
        
        internal System.Windows.Controls.StackPanel borderUserInformation;
        
        internal System.Windows.Controls.TextBlock textBlockPower;
        
        internal System.Windows.Controls.TextBlock textBlockMoney;
        
        internal System.Windows.Controls.TextBlock textBlockCurrent;
        
        internal System.Windows.Controls.TextBlock textBlockMax;
        
        internal System.Windows.Controls.ProgressBar progressBarEnergy;
        
        internal System.Windows.Controls.TextBlock textBlockEnergyCountDown;
        
        internal System.Windows.Controls.TextBlock textBlockUserLevel;
        
        internal System.Windows.Controls.ProgressBar progressBarUserLevel;
        
        internal System.Windows.Controls.DataGrid dataGridUserItems;
        
        internal System.Windows.Controls.Button buttonZoomToUser;
        
        internal System.Windows.Controls.Button buttonZoomIn;
        
        internal System.Windows.Controls.Button buttonZoomOut;
        
        internal System.Windows.Controls.Button buttonFullScreen;
        
        internal System.Windows.Controls.Grid gridLocationDetails;
        
        internal UndeadEarth.Silverlight.PopupUserControl popupUserControl;
        
        internal UndeadEarth.Silverlight.TutorialUserControl tutorialUserControl;
        
        internal UndeadEarth.Silverlight.LevelUpUserControl levelUpUserControl;
        
        internal UndeadEarth.Silverlight.AchievementUserControl achievementUserControl;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/UndeadEarth.Silverlight;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.rowForMap = ((System.Windows.Controls.RowDefinition)(this.FindName("rowForMap")));
            this.mapUndeadEarth = ((Microsoft.Maps.MapControl.Map)(this.FindName("mapUndeadEarth")));
            this.borderStatus = ((System.Windows.Controls.Border)(this.FindName("borderStatus")));
            this.textBlockStatus = ((System.Windows.Controls.TextBlock)(this.FindName("textBlockStatus")));
            this.textBlockUserName = ((System.Windows.Controls.TextBlock)(this.FindName("textBlockUserName")));
            this.borderUserInformation = ((System.Windows.Controls.StackPanel)(this.FindName("borderUserInformation")));
            this.textBlockPower = ((System.Windows.Controls.TextBlock)(this.FindName("textBlockPower")));
            this.textBlockMoney = ((System.Windows.Controls.TextBlock)(this.FindName("textBlockMoney")));
            this.textBlockCurrent = ((System.Windows.Controls.TextBlock)(this.FindName("textBlockCurrent")));
            this.textBlockMax = ((System.Windows.Controls.TextBlock)(this.FindName("textBlockMax")));
            this.progressBarEnergy = ((System.Windows.Controls.ProgressBar)(this.FindName("progressBarEnergy")));
            this.textBlockEnergyCountDown = ((System.Windows.Controls.TextBlock)(this.FindName("textBlockEnergyCountDown")));
            this.textBlockUserLevel = ((System.Windows.Controls.TextBlock)(this.FindName("textBlockUserLevel")));
            this.progressBarUserLevel = ((System.Windows.Controls.ProgressBar)(this.FindName("progressBarUserLevel")));
            this.dataGridUserItems = ((System.Windows.Controls.DataGrid)(this.FindName("dataGridUserItems")));
            this.buttonZoomToUser = ((System.Windows.Controls.Button)(this.FindName("buttonZoomToUser")));
            this.buttonZoomIn = ((System.Windows.Controls.Button)(this.FindName("buttonZoomIn")));
            this.buttonZoomOut = ((System.Windows.Controls.Button)(this.FindName("buttonZoomOut")));
            this.buttonFullScreen = ((System.Windows.Controls.Button)(this.FindName("buttonFullScreen")));
            this.gridLocationDetails = ((System.Windows.Controls.Grid)(this.FindName("gridLocationDetails")));
            this.popupUserControl = ((UndeadEarth.Silverlight.PopupUserControl)(this.FindName("popupUserControl")));
            this.tutorialUserControl = ((UndeadEarth.Silverlight.TutorialUserControl)(this.FindName("tutorialUserControl")));
            this.levelUpUserControl = ((UndeadEarth.Silverlight.LevelUpUserControl)(this.FindName("levelUpUserControl")));
            this.achievementUserControl = ((UndeadEarth.Silverlight.AchievementUserControl)(this.FindName("achievementUserControl")));
        }
    }
}

