﻿#pragma checksum "C:\Users\stijnvanhulle\Documents\GitHub\Project-App-dev-voor-windows\BOBApp\BOBApp\Views\Profiel.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "EDDDCD2F8D3261ACD16AA11F3159446B"
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
    partial class Profiel : 
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
                    this.modal = (global::Windows.UI.Xaml.Controls.RelativePanel)(target);
                }
                break;
            case 2:
                {
                    this.textBoxEmail = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                    #line 41 "..\..\..\Views\Profiel.xaml"
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.textBoxEmail).TextChanged += this.textBoxEmail_TextChanged;
                    #line default
                }
                break;
            case 3:
                {
                    this.textBoxGSMnr = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                    #line 42 "..\..\..\Views\Profiel.xaml"
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.textBoxGSMnr).TextChanged += this.textBoxGSMnr_TextChanged;
                    #line default
                }
                break;
            case 4:
                {
                    this.tglIsBob = (global::Windows.UI.Xaml.Controls.ToggleSwitch)(target);
                    #line 43 "..\..\..\Views\Profiel.xaml"
                    ((global::Windows.UI.Xaml.Controls.ToggleSwitch)this.tglIsBob).Toggled += this.tglIsBob_Toggled;
                    #line default
                }
                break;
            case 5:
                {
                    this.textBoxNummerplaat = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                    #line 44 "..\..\..\Views\Profiel.xaml"
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.textBoxNummerplaat).TextChanged += this.textBoxNummerplaat_TextChanged;
                    #line default
                }
                break;
            case 6:
                {
                    this.textBlockAutoType = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 7:
                {
                    this.comboBox = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                    #line 46 "..\..\..\Views\Profiel.xaml"
                    ((global::Windows.UI.Xaml.Controls.ComboBox)this.comboBox).SelectionChanged += this.comboBox_SelectionChanged;
                    #line default
                }
                break;
            case 8:
                {
                    this.textboxPrice = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                    #line 47 "..\..\..\Views\Profiel.xaml"
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.textboxPrice).TextChanged += this.textboxPrice_TextChanged;
                    #line default
                }
                break;
            case 9:
                {
                    this.textBlockBobType = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 10:
                {
                    this.comboBoxTypeBob = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                    #line 49 "..\..\..\Views\Profiel.xaml"
                    ((global::Windows.UI.Xaml.Controls.ComboBox)this.comboBoxTypeBob).SelectionChanged += this.comboBoxTypeBob_SelectionChanged;
                    #line default
                }
                break;
            case 11:
                {
                    this.buttonAanpassen = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 12:
                {
                    this.buttonWachtwoord = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 13:
                {
                    this.frame = (global::Windows.UI.Xaml.Controls.Frame)(target);
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

