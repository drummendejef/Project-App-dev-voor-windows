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

        private void Akkoord_checked(object sender, RoutedEventArgs e)//Gebruiker heeft op de "akkoord" radiobutton geklikt.
        {
            //Alle mogelijke fouten overlopen.
            if (textBoxNaam.Text.Length <= 2)
            {
                textBlockError.Text = "Vul een naam in";
                Debug.WriteLine("Er staat niets in de naam textbox");
                checkBoxAkkoordVoorwaarden.IsChecked = false;
            }
            else if (textBoxVoornaam.Text.Length <= 2)
            {
                textBlockError.Text = "Vul een Voornaam in";
                Debug.WriteLine("Er staat niets in de textboxVoornaam");
                checkBoxAkkoordVoorwaarden.IsChecked = false;
            }
            else if (textBoxEmail.Text.Length <= 5)
            {
                textBlockError.Text = "Vul een Emailadres in";
                Debug.WriteLine("Er staat niets in de Email textbox");
                checkBoxAkkoordVoorwaarden.IsChecked = false;
            }
            else if (textBoxGSMnr.Text.Length <= 4)
            {
                textBlockError.Text = "Vul een GSM nummer in";
                Debug.WriteLine("Er staat niets in de GSM nummer");
                checkBoxAkkoordVoorwaarden.IsChecked = false;
            }
            else
            {
                buttonRegistreer.IsEnabled = true;
                textBlockError.Text = "";
            }



#if DEBUG
            Debug.WriteLine("Akkoord checkbox checked");
#endif
        }

        private void Akkoord_unchecked(object sender, RoutedEventArgs e)//Gebruiker heeft de "akkoord" radiobutton ontklikt.
        {
            buttonRegistreer.IsEnabled = false;
        }

    }
}