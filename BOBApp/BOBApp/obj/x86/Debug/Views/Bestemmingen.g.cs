﻿#pragma checksum "C:\Users\stijnvanhulle\Documents\GitHub\Project-App-dev-voor-windows\BOBApp\BOBApp\Views\Bestemmingen.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "92D834704D9FE988ED6ED896C98DC6C2"
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
    partial class Bestemmingen : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        internal class XamlBindingSetters
        {
        };

        private class Bestemmingen_obj1_Bindings :
            global::Windows.UI.Xaml.Markup.IComponentConnector,
            IBestemmingen_Bindings
        {
            private global::BOBApp.Views.Bestemmingen dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);

            // Fields for each control that has bindings.
            private global::Windows.UI.Xaml.Controls.ListView obj7;
            private global::Windows.UI.Xaml.Controls.AutoSuggestBox obj9;

            public Bestemmingen_obj1_Bindings()
            {
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 7:
                        this.obj7 = (global::Windows.UI.Xaml.Controls.ListView)target;
                        ((global::Windows.UI.Xaml.Controls.ListView)target).SelectionChanged += (global::System.Object param0, global::Windows.UI.Xaml.Controls.SelectionChangedEventArgs param1) =>
                        {
                        this.dataRoot.Vm.Changed(param0, param1);
                        };
                        break;
                    case 9:
                        this.obj9 = (global::Windows.UI.Xaml.Controls.AutoSuggestBox)target;
                        ((global::Windows.UI.Xaml.Controls.AutoSuggestBox)target).SuggestionChosen += (global::Windows.UI.Xaml.Controls.AutoSuggestBox param0, global::Windows.UI.Xaml.Controls.AutoSuggestBoxSuggestionChosenEventArgs param1) =>
                        {
                        this.dataRoot.Vm.SearchChosen(param0, param1);
                        };
                        break;
                    default:
                        break;
                }
            }

            // IBestemmingen_Bindings

            public void Initialize()
            {
                if (!this.initialized)
                {
                    this.Update();
                }
            }
            
            public void Update()
            {
                this.initialized = true;
            }

            public void StopTracking()
            {
            }

            // Bestemmingen_obj1_Bindings

            public void SetDataRoot(global::BOBApp.Views.Bestemmingen newDataRoot)
            {
                this.dataRoot = newDataRoot;
            }

            public void Loading(global::Windows.UI.Xaml.FrameworkElement src, object data)
            {
                this.Initialize();
            }

            // Update methods for each path node used in binding steps.
        }
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2:
                {
                    this.VisualStateGroup = (global::Windows.UI.Xaml.VisualStateGroup)(target);
                }
                break;
            case 3:
                {
                    this.small = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 4:
                {
                    this.big = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 5:
                {
                    this.search = (global::Windows.UI.Xaml.Controls.RelativePanel)(target);
                }
                break;
            case 6:
                {
                    this.btnAdd = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 7:
                {
                    this.Friends = (global::Windows.UI.Xaml.Controls.ListView)(target);
                }
                break;
            case 8:
                {
                    this.innerSearch = (global::Windows.UI.Xaml.Controls.RelativePanel)(target);
                }
                break;
            case 9:
                {
                    this.textBoxZoekLocaties = (global::Windows.UI.Xaml.Controls.AutoSuggestBox)(target);
                }
                break;
            case 10:
                {
                    this.btnSearch = (global::Windows.UI.Xaml.Controls.Button)(target);
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
            switch(connectionId)
            {
            case 1:
                {
                    global::Windows.UI.Xaml.Controls.Page element1 = (global::Windows.UI.Xaml.Controls.Page)target;
                    Bestemmingen_obj1_Bindings bindings = new Bestemmingen_obj1_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot(this);
                    this.Bindings = bindings;
                    element1.Loading += bindings.Loading;
                }
                break;
            }
            return returnValue;
        }
    }
}

