﻿#pragma checksum "..\..\..\..\..\Layers\Tracks\CCTV\CCTVConfigForm.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "8A43F74EF6D185BE98043D7E929CE41F"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace VTSCore.Layers.Tracks.CCTV {
    
    
    /// <summary>
    /// CCTVConfigForm
    /// </summary>
    public partial class CCTVConfigForm : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\..\..\Layers\Tracks\CCTV\CCTVConfigForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbIp;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\..\..\Layers\Tracks\CCTV\CCTVConfigForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbCCTVMode;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\..\..\Layers\Tracks\CCTV\CCTVConfigForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btOk;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\..\..\Layers\Tracks\CCTV\CCTVConfigForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btQuit;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/TestTool;component/layers/tracks/cctv/cctvconfigform.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Layers\Tracks\CCTV\CCTVConfigForm.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 4 "..\..\..\..\..\Layers\Tracks\CCTV\CCTVConfigForm.xaml"
            ((VTSCore.Layers.Tracks.CCTV.CCTVConfigForm)(target)).Closed += new System.EventHandler(this.Window_Closed);
            
            #line default
            #line hidden
            return;
            case 2:
            this.tbIp = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.cbCCTVMode = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 4:
            this.btOk = ((System.Windows.Controls.Button)(target));
            
            #line 15 "..\..\..\..\..\Layers\Tracks\CCTV\CCTVConfigForm.xaml"
            this.btOk.Click += new System.Windows.RoutedEventHandler(this.btOk_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btQuit = ((System.Windows.Controls.Button)(target));
            
            #line 16 "..\..\..\..\..\Layers\Tracks\CCTV\CCTVConfigForm.xaml"
            this.btQuit.Click += new System.Windows.RoutedEventHandler(this.btQuit_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

