using BOBApp.ViewModels;
using System;
using System.Collections.Generic;
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
    public sealed partial class Profiel : Page
    {
        public ProfielVM Vm
        {
            get
            {
                return this.DataContext as ProfielVM;
            }
        }

        public Profiel()
        {
            this.InitializeComponent();

            Vm.Frame = frame;

            if (tglIsBob.IsOn)
            {
                textboxPrice.IsEnabled = true;
                textBoxNummerplaat.IsEnabled = true;
                comboBoxTypeBob.IsEnabled = true;
                comboBox.IsEnabled = true;
            }
            else
            {
                textboxPrice.IsEnabled = false;
                textBoxNummerplaat.IsEnabled = false;
                comboBoxTypeBob.IsEnabled = false;
                comboBox.IsEnabled = false;
            }
        }

        private void tglIsBob_Toggled(object sender, RoutedEventArgs e)
        {
             if (tglIsBob.IsOn)
            {
                textboxPrice.IsEnabled = true;
                textBoxNummerplaat.IsEnabled = true;
                comboBoxTypeBob.IsEnabled = true;
                comboBox.IsEnabled = true;

                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonAanpassen.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonAanpassen.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonAanpassen.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonAanpassen.IsEnabled = false;

                }
                else if(textBoxNummerplaat.Text.Length < 1)
                {
                    buttonAanpassen.IsEnabled = false;
                }
                else if(comboBox.SelectedIndex < 0)
                {
                    buttonAanpassen.IsEnabled = false;
                }
                else if(textboxPrice.Text.Length < 1)
                {
                    buttonAanpassen.IsEnabled = false;
                }
                else
                {
                    buttonAanpassen.IsEnabled = true;
                }
            }
            else
            {

                textboxPrice.IsEnabled = false;
                textBoxNummerplaat.IsEnabled = false;
                comboBoxTypeBob.IsEnabled = false;
                comboBox.IsEnabled = false;

                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonAanpassen.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonAanpassen.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonAanpassen.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonAanpassen.IsEnabled = false;

                }
                else
                {
                    buttonAanpassen.IsEnabled = true;
                }
            }
        }

        private void textBoxVoornaam_TextChanged(object sender, TextChangedEventArgs e)
        {
          
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonAanpassen.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonAanpassen.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonAanpassen.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonAanpassen.IsEnabled = false;

                }
                else
                {
                    buttonAanpassen.IsEnabled = true;
                }
            
        }

        private void textBoxNaam_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxNaam.Text.Length <= 2)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxVoornaam.Text.Length <= 1)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxEmail.Text.Length <= 5)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxGSMnr.Text.Length <= 4)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else
            {
                buttonAanpassen.IsEnabled = true;
            }
        }

        private void textBoxEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxNaam.Text.Length <= 2)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxVoornaam.Text.Length <= 1)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxEmail.Text.Length <= 5)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxGSMnr.Text.Length <= 4)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else
            {
                buttonAanpassen.IsEnabled = true;
            }
        }

        private void textBoxGSMnr_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxNaam.Text.Length <= 2)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxVoornaam.Text.Length <= 1)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxEmail.Text.Length <= 5)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxGSMnr.Text.Length <= 4)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else
            {
                buttonAanpassen.IsEnabled = true;
            }
        }

        private void textBoxNummerplaat_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxNaam.Text.Length <= 2)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxVoornaam.Text.Length <= 1)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxEmail.Text.Length <= 5)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxGSMnr.Text.Length <= 4)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxNummerplaat.Text.Length < 1)
            {
                buttonAanpassen.IsEnabled = false;
            }
            else if (comboBox.SelectedIndex < 0)
            {
                buttonAanpassen.IsEnabled = false;
            }
            else if (textboxPrice.Text.Length < 1)
            {
                buttonAanpassen.IsEnabled = false;
            }
            else if (comboBoxTypeBob.SelectedIndex < 0)
            {
                buttonAanpassen.IsEnabled = false;
            }
            else
            {
                buttonAanpassen.IsEnabled = true;
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (textBoxNaam.Text.Length <= 2)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxVoornaam.Text.Length <= 1)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxEmail.Text.Length <= 5)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxGSMnr.Text.Length <= 4)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxNummerplaat.Text.Length < 1)
            {
                buttonAanpassen.IsEnabled = false;
            }
            else if (comboBox.SelectedIndex < 0)
            {
                buttonAanpassen.IsEnabled = false;
            }
            else if (textboxPrice.Text.Length < 1)
            {
                buttonAanpassen.IsEnabled = false;
            }
            else if (comboBoxTypeBob.SelectedIndex < 0)
            {
                buttonAanpassen.IsEnabled = false;
            }
            else
            {
                buttonAanpassen.IsEnabled = true;
            }
        }

        private void textboxPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
           
                if (textBoxNaam.Text.Length <= 2)
                {
                    buttonAanpassen.IsEnabled = false;

                }
                else if (textBoxVoornaam.Text.Length <= 1)
                {
                    buttonAanpassen.IsEnabled = false;

                }
                else if (textBoxEmail.Text.Length <= 5)
                {
                    buttonAanpassen.IsEnabled = false;

                }
                else if (textBoxGSMnr.Text.Length <= 4)
                {
                    buttonAanpassen.IsEnabled = false;

                }
                else if (textBoxNummerplaat.Text.Length < 1)
                {
                    buttonAanpassen.IsEnabled = false;
                }
                else if (comboBox.SelectedIndex < 0)
                {
                    buttonAanpassen.IsEnabled = false;
                }
                else if (textboxPrice.Text.Length < 1)
                {
                    buttonAanpassen.IsEnabled = false;
                }
                else if (comboBoxTypeBob.SelectedIndex < 0)
                {
                    buttonAanpassen.IsEnabled = false;
                }
                else
                {
                    buttonAanpassen.IsEnabled = true;
                }
    
        }

        private void comboBoxTypeBob_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (textBoxNaam.Text.Length <= 2)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxVoornaam.Text.Length <= 1)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxEmail.Text.Length <= 5)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxGSMnr.Text.Length <= 4)
            {
                buttonAanpassen.IsEnabled = false;

            }
            else if (textBoxNummerplaat.Text.Length < 1)
            {
                buttonAanpassen.IsEnabled = false;
            }
            else if (comboBox.SelectedIndex < 0)
            {
                buttonAanpassen.IsEnabled = false;
            }
            else if (textboxPrice.Text.Length < 1)
            {
                buttonAanpassen.IsEnabled = false;
            }
            else if (comboBoxTypeBob.SelectedIndex < 0)
            {
                buttonAanpassen.IsEnabled = false;
            }
            else
            {
                buttonAanpassen.IsEnabled = true;
            }
        }
    }
}
