﻿#pragma checksum "c:\Users\juwilliams\Desktop\Personal\Undead-Planet\UndeadEarth.Silverlight\ZombiePackProgressUserControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F1BBAFBBFF1267B3BA878A2284602566"
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
    
    
    public partial class ZombiePackProgressUserControl : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.StackPanel LayoutRoot;
        
        internal System.Windows.Controls.Button buttonHunt;
        
        internal System.Windows.Controls.ProgressBar progressBarDestructionProgress;
        
        internal System.Windows.Controls.TextBlock textBlockPercent;
        
        internal System.Windows.Controls.TextBlock textBlockEnergy;
        
        internal System.Windows.Controls.TextBlock textBlockAttackPower;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/UndeadEarth.Silverlight;component/ZombiePackProgressUserControl.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.StackPanel)(this.FindName("LayoutRoot")));
            this.buttonHunt = ((System.Windows.Controls.Button)(this.FindName("buttonHunt")));
            this.progressBarDestructionProgress = ((System.Windows.Controls.ProgressBar)(this.FindName("progressBarDestructionProgress")));
            this.textBlockPercent = ((System.Windows.Controls.TextBlock)(this.FindName("textBlockPercent")));
            this.textBlockEnergy = ((System.Windows.Controls.TextBlock)(this.FindName("textBlockEnergy")));
            this.textBlockAttackPower = ((System.Windows.Controls.TextBlock)(this.FindName("textBlockAttackPower")));
        }
    }
}
