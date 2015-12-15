using BOBApp.Messages;
using BOBApp.ViewModels;
using BOBApp.Views;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using Microsoft.QueryStringDotNET;
using Windows.System;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.ApplicationModel.Core;
using System.Threading.Tasks;
using Windows.UI.Popups;


namespace BOBApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        //Globale variabele (positie) van de eigenaar
        public Geoposition UserLocation { get; set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;

          
         
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

            Messenger.Default.Register<GoToPage>(this, NavigateToPage);
            Messenger.Default.Register<Dialog>(this, DialogChange);

            Frame rootFrame = Window.Current.Content as Frame;

            //color titlebar
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Color.FromArgb(255, 62, 93, 148);
            titleBar.ForegroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = Color.FromArgb(255, 62, 93, 148);
            titleBar.ButtonForegroundColor = Colors.White;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;


               

            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter

                //Zolang navigatie niet volledig op punt staat, views testen door typeof() aan te passen naar de view
                rootFrame.Navigate(typeof(Login), e.Arguments);//Zorgen dat het startscherm de Login is.

                //rootFrame.Navigate(typeof(Bestemmingen), e.Arguments);//Zorgen dat het bestemmingscherm het startscherm is (als test)
                //rootFrame.Navigate(typeof(Register), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        private async void DialogChange(Dialog dialog)
        {
            if (dialog.Message != null)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
               async () =>
               {
                   Task task = ShowDialog(dialog.Message, dialog.Ok, dialog.Nok, dialog.ViewOk, dialog.ViewNok, dialog.ParamView, dialog.Cb);

               });
            }

        }
        private async Task<bool> ShowDialog(string text, string ok, string nok, Type viewOk, Type viewNok, bool paramView, string cb)
        {
            var dialog = new MessageDialog(text);
            var test2Command = nok;

            if (ok == null) ok = "Ok";
            if (ok == null && nok == null)
            {
                ok = "Yes"; nok = "No";
            }


            if (test2Command != null)
            {
                dialog.Commands.Add(new UICommand(ok) { Id = 0 });
                dialog.Commands.Add(new UICommand(nok) { Id = 1 });

                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;
            }
            else
            {
                dialog.Commands.Add(new UICommand(ok) { Id = 0 });

                dialog.DefaultCommandIndex = 0;
            }


            try
            {
                var result = await dialog.ShowAsync();
  

                int id = int.Parse(result.Id.ToString());
                if (id == 0)
                {
                    if (viewOk != null)
                    {
                        Frame rootFrame = MainViewVM.MainFrame as Frame;
                        rootFrame.Navigate(viewOk, paramView);

                    }
                    if (cb != null)
                    {
                        Messenger.Default.Send<NavigateTo>(new NavigateTo()
                        {
                            Name = cb,
                            Result = true
                        });
                    }

                    return true;
                }
                else
                {
                    if (viewOk != null)
                    {
                        Frame rootFrame = MainViewVM.MainFrame as Frame;
                        rootFrame.Navigate(viewNok, paramView);

                    }

                    if (cb != null)
                    {
                        Messenger.Default.Send<NavigateTo>(new NavigateTo()
                        {
                            Name = cb,
                            Result = false
                        });
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                var test = ex.Message;
                throw;
            }

            

            
        }


        private void NavigateToPage(GoToPage message)
        {
            Frame rootFrame = Window.Current.Content as Frame;
         
            switch (message.Name)
            {
                case "Bestemmingen":
                    rootFrame.Navigate(typeof(Bestemmingen));
                    break;
                case "Bestemmingen_Nieuw":
                    rootFrame.Navigate(typeof(Bestemmingen_Nieuw));
                    break;
                case "FeestjesOverzicht":
                    rootFrame.Navigate(typeof(FeestjesOverzicht));
                    break;
                case "Login":
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    rootFrame.Navigate(typeof(Login));
                    break;
                case "MainView":
                    rootFrame.Navigate(typeof(MainView));
                    break;
                case "Profiel":
                    rootFrame.Navigate(typeof(Profiel));
                    break;
                case "Punten":
                    rootFrame.Navigate(typeof(Punten));
                    break;
                case "Register":
                    rootFrame.Navigate(typeof(Register));
                    break;
                case "MijnRitten":
                    rootFrame.Navigate(typeof(MijnRitten));
                    break;
                case "VindRit":
                    rootFrame.Navigate(typeof(VindRit));
                    break;
                case "VindRitFilter":
                    rootFrame.Navigate(typeof(VindRitFilter));
                    break;
                case "VindRitChat":
                    rootFrame.Navigate(typeof(VindRitChat));
                    break;
                case "VindRitBob":
                    rootFrame.Navigate(typeof(VindRitBob));
                    break;
                case "ZoekVrienden":
                    rootFrame.Navigate(typeof(ZoekVrienden));
                    break;
                default:
                    rootFrame.Navigate(typeof(Login));
                    break;
            }
        }



        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
          
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        //Als de gebruiker op de toast/notification klikt
        protected override async void OnActivated(IActivatedEventArgs e)
        {
            //De rootframe ophalen
            Frame rootFrame = Window.Current.Content as Frame;

            //TODO: initialiseren van root frame, net als in OnLaunched

            //Toast Activation opvangen
            if (e is ToastNotificationActivatedEventArgs)
            {
                var toastActivationArgs = e as ToastNotificationActivatedEventArgs;

                QueryString args = QueryString.Parse(toastActivationArgs.Argument);

                //Kijk welke actie gevraagd is
                switch (args["action"])
                {
                    //Open de application
                    case "openBobApp":
                        //Nog uitzoeken hoe je dat moet doen, nog niet zo heel belangrijk
                        break;
                    //Open het scherm waar je toestemming geeft om je locatie te gebruiken.
                    case "openLocationServices":
                        await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
                        break;
                }
            }

            //Maak zeker dat het scherm nu actief is
            Window.Current.Activate();
        }

        //Als de status van de locatie permissies veranderd is.
        async private void OnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            //TODO: Locatie opvragen afwerken?
            //  https://msdn.microsoft.com/en-us/library/windows/desktop/mt219698.aspx

            /*await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

            });*/
            Debug.WriteLine("Status Locatie toestemming is veranderd");

            switch (args.Status)
            {
                case PositionStatus.Ready:
                    //We krijgen locatie data binnen
                    //aanmaken Geolocator
                    /*Geolocator geolocator = new Geolocator();

                    //Inschrijven op de StatusChanged voor updates van de permissies voor locaties.
                    geolocator.StatusChanged += OnStatusChanged;

                    //Locatie opvragen
                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    Debug.WriteLine("Positie opgevraagd, lat: " + pos.Coordinate.Point.Position.Latitude + " lon: " + pos.Coordinate.Point.Position.Longitude);

                    //Locatie opslaan als gebruikerslocatie
                    (App.Current as App).UserLocation = pos;*/
                    Debug.WriteLine("Positionstatus Ready");

                    break;
                case PositionStatus.Initializing:
                    Debug.WriteLine("Positionstatus Initializing");
                    break;
                case PositionStatus.Disabled:
                    Debug.WriteLine("Positionstatus Disabled");
                    break;
            }
        }


       
    }
}
