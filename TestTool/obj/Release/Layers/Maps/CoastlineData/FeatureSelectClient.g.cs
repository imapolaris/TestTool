﻿#pragma checksum "..\..\..\..\..\Layers\Maps\CoastlineData\FeatureSelectClient.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E66D0B6473FFCD2A06DA02D761129CBC"
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


namespace TestTool.Layers.Maps {
    
    
    /// <summary>
    /// FeatureSelectClient
    /// </summary>
    public partial class FeatureSelectClient : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 7 "..\..\..\..\..\Layers\Maps\CoastlineData\FeatureSelectClient.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbShowAll;
        
        #line default
        #line hidden
        
        
        #line 8 "..\..\..\..\..\Layers\Maps\CoastlineData\FeatureSelectClient.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView featureSelectedListView;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\..\..\Layers\Maps\CoastlineData\FeatureSelectClient.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ContextMenu mouseRightList;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\..\..\Layers\Maps\CoastlineData\FeatureSelectClient.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem centeredMenu;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\..\..\Layers\Maps\CoastlineData\FeatureSelectClient.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem ExportGIMenu;
        
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
            System.Uri resourceLocater = new System.Uri("/TestTool;component/layers/maps/coastlinedata/featureselectclient.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Layers\Maps\CoastlineData\FeatureSelectClient.xaml"
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
            
            #line 5 "..\..\..\..\..\Layers\Maps\CoastlineData\FeatureSelectClient.xaml"
            ((TestTool.Layers.Maps.FeatureSelectClient)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.cbShowAll = ((System.Windows.Controls.CheckBox)(target));
            
            #line 7 "..\..\..\..\..\Layers\Maps\CoastlineData\FeatureSelectClient.xaml"
            this.cbShowAll.Click += new System.Windows.RoutedEventHandler(this.cbShowAll_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.featureSelectedListView = ((System.Windows.Controls.ListView)(target));
            
            #line 8 "..\..\..\..\..\Layers\Maps\CoastlineData\FeatureSelectClient.xaml"
            this.featureSelectedListView.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.listView_MouseDoubleClick);
            
            #line default
            #line hidden
            
            #line 8 "..\..\..\..\..\Layers\Maps\CoastlineData\FeatureSelectClient.xaml"
            this.featureSelectedListView.PreviewMouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.featureSelectedListView_PreviewMouseRightButtonDown);
            
            #line default
            #line hidden
            
            #line 8 "..\..\..\..\..\Layers\Maps\CoastlineData\FeatureSelectClient.xaml"
            this.featureSelectedListView.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.featureSelectedListView_MouseRightButtonDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.mouseRightList = ((System.Windows.Controls.ContextMenu)(target));
            return;
            case 5:
            this.centeredMenu = ((System.Windows.Controls.MenuItem)(target));
            
            #line 11 "..\..\..\..\..\Layers\Maps\CoastlineData\FeatureSelectClient.xaml"
            this.centeredMenu.Click += new System.Windows.RoutedEventHandler(this.centeredMenu_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.ExportGIMenu = ((System.Windows.Controls.MenuItem)(target));
            
            #line 12 "..\..\..\..\..\Layers\Maps\CoastlineData\FeatureSelectClient.xaml"
            this.ExportGIMenu.Click += new System.Windows.RoutedEventHandler(this.ExportGIMenu_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

