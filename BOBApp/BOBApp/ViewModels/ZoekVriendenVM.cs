using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
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

namespace BOBApp.ViewModels
{
    public class ZoekVriendenVM : ViewModelBase
    {
        //Properties
        public bool Loading { get; set; }

        //Constructor
        public ZoekVriendenVM()
        {
            AddFriend("1@bob.com");

            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);
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


                    GetFriends();
                    this.Loading = false;
                    RaisePropertyChanged("Loading");


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
                User.All user = (User.All)obj.Result2;
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

        private async void AddFriend(string email)
        {
            email = email.Trim().ToLower();


            User.All user = Task.FromResult<User.All>(await UsersRepository.GetUserByEmail(email)).Result;
            Libraries.Socket socketSend = new Libraries.Socket() { From = MainViewVM.USER.ID, To = user.User.ID, Status = true };

            MainViewVM.socket.Emit("friend_REQUEST:send", JsonConvert.SerializeObject(socketSend));
        }

        private void GetFriends()
        {
            //
        }

    }
}
