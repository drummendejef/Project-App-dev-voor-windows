﻿#pragma checksum "C:\Users\stijnvanhulle\Documents\GitHub\Project-App-dev-voor-windows\BOBApp\BOBApp\Views\MainView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "50F283D43B0DD99353872CC4FA574B8D"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BOBApp.Views
{
    partial class MainView : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.ShellSplitView = (global::Windows.UI.Xaml.Controls.SplitView)(target);
                    #line 19 "..\..\..\Views\MainView.xaml"
                    ((global::Windows.UI.Xaml.Controls.SplitView)this.ShellSplitView).PaneClosing += this.ShellSplitView_PaneClosing;
                    #line default
                }
                break;
            case 2:
                {
                    global::Windows.UI.Xaml.Controls.RadioButton element2 = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 24 "..\..\..\Views\MainView.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)element2).Click += this.OnMenuButtonClicked;
                    #line default
                }
                break;
            case 3:
                {
                    this.user = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 4:
                {
                    this.bobVindEenRit = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 43 "..\..\..\Views\MainView.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)this.bobVindEenRit).Checked += this.OnVindEenRitChecked;
                    #line default
                }
                break;
            case 5:
                {
                    this.bobBiedJeAan = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 44 "..\..\..\Views\MainView.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)this.bobBiedJeAan).Checked += this.OnBiedJeAanChecked;
                    #line default
                }
                break;
            case 6:
                {
                    global::Windows.UI.Xaml.Controls.RadioButton element6 = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 46 "..\..\..\Views\MainView.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)element6).Checked += this.OnVindVriendenChecked;
                    #line default
                }
                break;
            case 7:
                {
                    global::Windows.UI.Xaml.Controls.RadioButton element7 = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 47 "..\..\..\Views\MainView.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)element7).Checked += this.OnMijnRittenChecked;
                    #line default
                }
                break;
            case 8:
                {
                    global::Windows.UI.Xaml.Controls.RadioButton element8 = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 48 "..\..\..\Views\MainView.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)element8).Checked += this.OnFeestjesInDeBuurtChecked;
                    #line default
                }
                break;
            case 9:
                {
                    global::Windows.UI.Xaml.Controls.RadioButton element9 = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 49 "..\..\..\Views\MainView.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)element9).Checked += this.OnBestemmingenChecked;
                    #line default
                }
                break;
            case 10:
                {
                    global::Windows.UI.Xaml.Controls.RadioButton element10 = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 50 "..\..\..\Views\MainView.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)element10).Checked += this.OnProfielChecked;
                    #line default
                }
                break;
            case 11:
                {
                    this.Change = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 51 "..\..\..\Views\MainView.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)this.Change).Click += this.OnChangeChecked;
                    #line 51 "..\..\..\Views\MainView.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)this.Change).Checked += this.OnProfielChecked;
                    #line default
                }
                break;
            case 12:
                {
                    global::Windows.UI.Xaml.Controls.TextBlock element12 = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                    #line 38 "..\..\..\Views\MainView.xaml"
                    ((global::Windows.UI.Xaml.Controls.TextBlock)element12).Tapped += this.TextBlock_Tapped;
                    #line default
                }
                break;
            case 13:
                {
                    this.frame = (global::Windows.UI.Xaml.Controls.Frame)(target);
                    #line 57 "..\..\..\Views\MainView.xaml"
                    ((global::Windows.UI.Xaml.Controls.Frame)this.frame).Navigated += this.frame_Navigated;
                    #line 57 "..\..\..\Views\MainView.xaml"
                    ((global::Windows.UI.Xaml.Controls.Frame)this.frame).Navigating += this.frame_Navigating;
                    #line 57 "..\..\..\Views\MainView.xaml"
                    ((global::Windows.UI.Xaml.Controls.Frame)this.frame).NavigationStopped += this.frame_NavigationStopped;
                    #line default
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

