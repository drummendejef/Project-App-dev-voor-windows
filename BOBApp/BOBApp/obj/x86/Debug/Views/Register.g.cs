﻿#pragma checksum "C:\Users\stijnvanhulle\Documents\GitHub\Project-App-dev-voor-windows\BOBApp\BOBApp\Views\Register.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "05F3C4C88D993F12CAB272A015E1C429"
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
    partial class Register : 
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
                    this.thisPage = (global::Windows.UI.Xaml.Controls.Page)(target);
                }
                break;
            case 2:
                {
                    this.VisualStateGroup = (global::Windows.UI.Xaml.VisualStateGroup)(target);
                }
                break;
            case 3:
                {
                    this.Under800 = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 4:
                {
                    this.Over800 = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 5:
                {
                    this.textBlockError = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 6:
                {
                    this.textBoxVoornaam = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                    #line 49 "..\..\..\Views\Register.xaml"
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.textBoxVoornaam).TextChanged += this.textBoxVoornaam_TextChanged;
                    #line default
                }
                break;
            case 7:
                {
                    this.textBoxNaam = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                    #line 50 "..\..\..\Views\Register.xaml"
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.textBoxNaam).TextChanged += this.textBoxNaam_TextChanged;
                    #line default
                }
                break;
            case 8:
                {
                    this.textBoxEmail = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                    #line 51 "..\..\..\Views\Register.xaml"
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.textBoxEmail).TextChanged += this.textBoxEmail_TextChanged;
                    #line default
                }
                break;
            case 9:
                {
                    this.textBoxGSMnr = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                    #line 52 "..\..\..\Views\Register.xaml"
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.textBoxGSMnr).TextChanged += this.textBoxGSMnr_TextChanged;
                    #line default
                }
                break;
            case 10:
                {
                    this.textBoxNummerplaat = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                    #line 53 "..\..\..\Views\Register.xaml"
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.textBoxNummerplaat).TextChanged += this.textBoxNummerplaat_TextChanged;
                    #line default
                }
                break;
            case 11:
                {
                    this.textBlockAutoType = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 12:
                {
                    this.comboBox = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                    #line 55 "..\..\..\Views\Register.xaml"
                    ((global::Windows.UI.Xaml.Controls.ComboBox)this.comboBox).SelectionChanged += this.comboBox_SelectionChanged;
                    #line default
                }
                break;
            case 13:
                {
                    this.textboxPrice = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                    #line 56 "..\..\..\Views\Register.xaml"
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.textboxPrice).TextChanged += this.textboxPrice_TextChanged;
                    #line default
                }
                break;
            case 14:
                {
                    this.tglIsBob = (global::Windows.UI.Xaml.Controls.ToggleSwitch)(target);
                    #line 57 "..\..\..\Views\Register.xaml"
                    ((global::Windows.UI.Xaml.Controls.ToggleSwitch)this.tglIsBob).Toggled += this.tglIsBob_Toggled;
                    #line default
                }
                break;
            case 15:
                {
                    this.passwordBox = (global::Windows.UI.Xaml.Controls.PasswordBox)(target);
                }
                break;
            case 16:
                {
                    this.passwordBoxHerhaal = (global::Windows.UI.Xaml.Controls.PasswordBox)(target);
                }
                break;
            case 17:
                {
                    this.buttonCancel = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 18:
                {
                    this.buttonRegistreer = (global::Windows.UI.Xaml.Controls.Button)(target);
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

