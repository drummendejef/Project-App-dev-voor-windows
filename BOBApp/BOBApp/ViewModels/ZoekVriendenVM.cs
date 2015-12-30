using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries.Models;
using Libraries.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BOBApp.ViewModels
{
    public class ZoekVriendenVM : ViewModelBase
    {
        //public static Frame Frame;
        //Properties
        public bool Loading { get; set; }
        public string Error { get; set; }


        public Visibility VisibleModal { get; set; }
        public Frame Frame { get; set; }
        public string SearchUser { get; set; }

        public RelayCommand AddFriendCommand { get; set; }
        public RelayCommand ShowModalCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand CloseModalCommand { get; set; }
     
        //Constructor
        public ZoekVriendenVM()
        {
           
          
            AddFriendCommand = new RelayCommand(AddFriend);
            CloseModalCommand = new RelayCommand(CloseModal);
            ShowModalCommand = new RelayCommand(ShowModal);
            SearchCommand = new RelayCommand(Search);

            VisibleModal = Visibility.Collapsed;

            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);
            RaiseAll();
        }

      

        private void RaiseAll()
        {
            RaisePropertyChanged("VisibleModal");
            try
            {
                RaisePropertyChanged("Frame");
            }
            catch (Exception ex)
            {

                
            }
            RaisePropertyChanged("SearchUsers");
            RaisePropertyChanged("Loading");
            RaisePropertyChanged("Error");

        }

        private async void Loaded()
        {
            this.Loading = true;
            RaisePropertyChanged("Loading");



            await Task.Run(async () =>
            {
                // running in background
#pragma warning disable CS1998
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {

                    VisibleModal = Visibility.Collapsed;

                    this.Frame = new Frame();


                    GetFriends();
                    this.Loading = false;
                    RaiseAll();




                });
#pragma warning restore CS1998

            });
        }

      

        private async void ExecuteNavigatedTo(NavigateTo obj)
        {
            if (obj.Name == "loaded")
            {
                Type view = (Type)obj.View;
                if (view == (typeof(ZoekVrienden)))
                {
                    //loaded
                    Loaded();
                }
            }

            if (obj.Name == "friend_accepted")
            {
                User.All user = (User.All)obj.ParamView;
                bool accepted = (bool) obj.Result;
                Response response = Task.FromResult<Response>(await FriendsRepository.PostFriend(MainViewVM.USER.ID,user.User.ID,accepted)).Result;

                if (response.Success == true)
                {
                    Messenger.Default.Send<Dialog>(new Dialog()
                    {
                        Message = user.ToString() + " is toegevoegd",
                    });
                }
                else
                {
                    //iets misgelopen
                }
               
              
            }
        }



        //Methods

        #region modal
        private void CloseModal()
        {
            VisibleModal = Visibility.Collapsed;
            RaiseAll();
        }

        private void ShowModal()
        {
            this.Frame = new Frame();
            this.Frame.Navigate(typeof(ZoekVrienden_Add));
           

            VisibleModal = Visibility.Visible;
            RaiseAll();

        }
        #endregion

        private void Search()
        {
            FindUserByEmail(this.SearchUser.Trim());
        }

      

        private void GetFriends()
        {
            //
        }


        #region add friend
       

        private async void FindUserByEmail(string email)
        {
            User.All item = await UsersRepository.GetUserByEmail(email);
            if (item != null)
            {
                if (this.SearchUsers != null)
                {
                    this.SearchUsers.Clear();
                }
              
                this.SearchUsers = new List<User.All>();
                this.SearchUsers.Add(item);
            }
            else
            {
                
                this.SearchUsers = await UsersRepository.GetUsersOnline();
               
               
            }

            this.SearchUsers = this.SearchUsers.Where(r => r.User.ID != MainViewVM.USER.ID).ToList<User.All>();
            if(this.SearchUsers.Count==0)
            {
                this.Error = "Geen gebruikers gevonden met dit email";
            }
            else
            {
                this.Error = null;
            }
            RaiseAll();
        }

        public List<User.All> SearchUsers { get; set; }
        public User.All SelectedUser { get; set; }
        private void AddFriend()
        {
            //todo: friends api
            User.All user = this.SelectedUser;

            Libraries.Socket socketSend = new Libraries.Socket() { From = MainViewVM.USER.ID, To = user.User.ID, Status = true };

            MainViewVM.socket.Emit("friend_REQUEST:send", JsonConvert.SerializeObject(socketSend));
            CloseModal();

            Messenger.Default.Send<Dialog>(new Dialog()
            {
                Message = "Vrienddschapsverzoek is verzonden",
                Ok = "Ok"
            });
        }



        #endregion



        //bind events
        /*public async void Changed(object sender, SelectionChangedEventArgs e)
        {
            ListView item = (ListView)sender;
            if (item.SelectedIndex == -1)
            {
                return;
            }


            var dialog = new ContentDialog()
            {
                Title = "",
            };

            // Setup Content
            var panel = new StackPanel();

            panel.Children.Add(new TextBlock
            {
                Text = "Volgende bestemming wilt u wijzigen: ",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 15)
            });

            var cb = new TextBox
            {
                TextWrapping = TextWrapping.Wrap,
                HorizontalContentAlignment = HorizontalAlignment.Stretch
            };



            panel.Children.Add(cb);
            dialog.Content = panel;

            // Add Buttons
            dialog.PrimaryButtonText = "Ok";
            dialog.PrimaryButtonClick += async delegate
            {
                string text = cb.Text;

                Loaded();
            };

            dialog.SecondaryButtonText = "Cancel";
            dialog.SecondaryButtonClick += delegate
            {

            };

            // Show Dialog
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.None)
            {

            }
            item.SelectedIndex = -1;


        }

    */


    }
}
