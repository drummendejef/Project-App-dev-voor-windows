using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BOBApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Register : Page
    {
        public Register()
        {
            this.InitializeComponent();
        }

        private void textBoxNaam_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tglIsBob.IsOn)
            {
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxNummerplaat.Text.Length < 1)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else if (comboBox.SelectedIndex < 0)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else if (textboxPrice.Text.Length < 1)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else
                {
                    buttonRegistreer.IsEnabled = true;
                }
            }
            else
            {
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else
                {
                    buttonRegistreer.IsEnabled = true;
                }
            }
        }

        private void textBoxVoornaam_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tglIsBob.IsOn)
            {
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxNummerplaat.Text.Length < 1)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else if (comboBox.SelectedIndex < 0)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else if (textboxPrice.Text.Length < 1)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else
                {
                    buttonRegistreer.IsEnabled = true;
                }
            }
            else
            {
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else
                {
                    buttonRegistreer.IsEnabled = true;
                }
            }
        }

        private void textBoxEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tglIsBob.IsOn)
            {
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxNummerplaat.Text.Length < 1)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else if (comboBox.SelectedIndex < 0)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else if (textboxPrice.Text.Length < 1)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else
                {
                    buttonRegistreer.IsEnabled = true;
                }
            }
            else
            {
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else
                {
                    buttonRegistreer.IsEnabled = true;
                }
            }
        }

        private void textBoxGSMnr_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tglIsBob.IsOn)
            {
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxNummerplaat.Text.Length < 1)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else if (comboBox.SelectedIndex < 0)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else if (textboxPrice.Text.Length < 1)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else
                {
                    buttonRegistreer.IsEnabled = true;
                }
            }
            else
            {
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else
                {
                    buttonRegistreer.IsEnabled = true;
                }
            }
        }

        private void textBoxNummerplaat_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tglIsBob.IsOn)
            {
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxNummerplaat.Text.Length < 1)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else if (comboBox.SelectedIndex < 0)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else if (textboxPrice.Text.Length < 1)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else
                {
                    buttonRegistreer.IsEnabled = true;
                }
            }
            else
            {
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else
                {
                    buttonRegistreer.IsEnabled = true;
                }
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tglIsBob.IsOn)
            {
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxNummerplaat.Text.Length < 1)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else if (comboBox.SelectedIndex < 0)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else if (textboxPrice.Text.Length < 1)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else
                {
                    buttonRegistreer.IsEnabled = true;
                }
            }
            else
            {
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else
                {
                    buttonRegistreer.IsEnabled = true;
                }
            }
        }

        private void tglIsBob_Toggled(object sender, RoutedEventArgs e)
        {
            if (tglIsBob.IsOn)
            {
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxNummerplaat.Text.Length < 1)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else if (comboBox.SelectedIndex < 0)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else if(textboxPrice.Text.Length < 1)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else
                {
                    buttonRegistreer.IsEnabled = true;
                }
            }
            else
            {
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else
                {
                    buttonRegistreer.IsEnabled = true;
                }
            }
        }

        private void textboxPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tglIsBob.IsOn)
            {
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxNummerplaat.Text.Length < 1)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else if (comboBox.SelectedIndex < 0)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else if (textboxPrice.Text.Length < 1)
                {
                    buttonRegistreer.IsEnabled = false;
                }
                else
                {
                    buttonRegistreer.IsEnabled = true;
                }
            }
            else
            {
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonRegistreer.IsEnabled = false;

                }
                else
                {
                    buttonRegistreer.IsEnabled = true;
                }
            }
        }
    }
}