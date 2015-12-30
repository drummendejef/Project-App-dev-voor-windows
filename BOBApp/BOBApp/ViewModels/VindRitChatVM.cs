using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries;
using Libraries.Models;
using Libraries.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace BOBApp.ViewModels
{
    public class VindRitChatVM : ViewModelBase
    {
        //Properties

        public static int? ID;


        public bool Loading { get; set; }
        public string Error { get; set; }



        public string SearchLocation { get; set; }
        public ChatRoom.All ChatRoom { get; set; }
        public RelayCommand AddCommentCommand { get; set; }
        public string ChatComment { get; set; }

       

        //Constructor
        public VindRitChatVM()
        {
            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);

            AddCommentCommand = new RelayCommand(AddComment);


           
            RaiseAll();
        }

        private void RaiseAll()
        {
            RaisePropertyChanged("ChatComment");
            RaisePropertyChanged("SearchLocation");
            RaisePropertyChanged("ChatRoom");
            RaisePropertyChanged("Loading");
            RaisePropertyChanged("Error");
        }

        private void ExecuteNavigatedTo(NavigateTo obj)
        {
            if (obj.Reload == true)
            {
                getChatroom();
            }


            if (obj.Name == "loaded")
            {
                Type view = (Type)obj.View;
                if (view == (typeof(VindRitChat)))
                {
                    Loaded();
                }
            }

            switch (obj.Name)
            {
                case "newComment":
                    getChatroom();



                    break;
                default:
                    break;
            }
        }

        private async void Loaded()
        {
            this.Loading = true;
            RaisePropertyChanged("Loading");

            await Task.Run(async () =>
            {
                // running in background
                getChatroom();
#pragma warning disable CS1998
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    this.Loading = false;
                    RaiseAll();

                });
#pragma warning restore CS1998

            });
        }




        //Methods
        private async void getChatroom()
        {
           
            try
            {

                if (VindRitChatVM.ID == null)
                {
                    string json = await Localdata.read("chatroom.json");
                    var definition = new { ID = 0, UserID=0 };
                    var data = JsonConvert.DeserializeAnonymousType(json, definition);
                    if (data.ID == -1)
                    {
                        //geen chatroom
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {

                            Type type = null;
                            if (MainViewVM.USER.IsBob == true)
                            {
                                type = typeof(VindRitBob);
                            }
                            else
                            {
                                type = typeof(VindRit);
                            }
                            Frame rootFrame = MainViewVM.MainFrame as Frame;
                            rootFrame.Navigate(type);
                        });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

                    }
                    else
                    {
                        VindRitChatVM.ID = data.ID;
                        MainViewVM.ChatRoom = new ChatRoom() { ID = VindRitChatVM.ID.Value };

                      
                        GetChatComments();

                     
                    }
                }
                else
                {
                    MainViewVM.ChatRoom = new ChatRoom() { ID = VindRitChatVM.ID.Value };

                   
                    GetChatComments();


                }
                   

            }
            catch (Exception ex)
            {

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    Type type = null;
                    if (MainViewVM.USER.IsBob == true)
                    {
                        type = typeof(VindRitBob);
                    }
                    else
                    {
                        type = typeof(VindRit);
                    }
                    Frame rootFrame = MainViewVM.MainFrame as Frame;
                    rootFrame.Navigate(type);

                });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            }

           



            



        }


        private async void AddComment()
        {
            //add comment to db
            await AddComment_task();
        }

        private async Task<Boolean> AddComment_task()
        {
            ChatComment comment = new ChatComment()
            {
                ChatRooms_ID= MainViewVM.ChatRoom.ID,
                Comment=this.ChatComment
            };
            Response res = await ChatRoomRepository.PostChatComment(comment);
            if (res.Success == true)
            {

                int bobID=ChatRoom.ChatRoom.Bobs_ID;
                Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(bobID)).Result;
                int userID = ChatRoom.ChatRoom.Users_ID;
                Libraries.Socket socketSendToBob;
                Libraries.Socket socketSendToUser;

                if (MainViewVM.USER.ID == userID)
                {
                    socketSendToBob = new Libraries.Socket()
                    {
                        From = MainViewVM.USER.ID,
                        To = bob.User.ID,
                        Status = true,
                        Object = comment.Comment,
                        Object2 = true
                    };
                    socketSendToUser = new Libraries.Socket()
                    {
                        From = bob.User.ID,
                        To = MainViewVM.USER.ID,
                        Status = true,
                        Object = comment.Comment,
                        Object2=false
                    };
                }
                else
                {
                    socketSendToBob = new Libraries.Socket()
                    {
                        From = MainViewVM.USER.ID,
                        To = bob.User.ID,
                        Status = true,
                        Object = comment.Comment,
                        Object2 = false
                    };
                    socketSendToUser = new Libraries.Socket()
                    {
                        From = bob.User.ID,
                        To = MainViewVM.USER.ID,
                        Status = true,
                        Object = comment.Comment,
                        Object2 = true
                    };
                }

              
                

                MainViewVM.socket.Emit("chatroom_COMMENT:send", JsonConvert.SerializeObject(socketSendToUser));
                MainViewVM.socket.Emit("chatroom_COMMENT:send", JsonConvert.SerializeObject(socketSendToBob));

                this.ChatComment = "";
                RaiseAll();
               
            }
            return res.Success;
        }

        private async void GetChatComments()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                ChatRoom.All lijst = await ChatRoomRepository.GetChatRoom(MainViewVM.ChatRoom.ID);

                if (lijst.ChatComments != null)
                {
                    for (int i = 0; i < lijst.ChatComments.Count; i++)
                    {
                        HorizontalAlignment aligment = HorizontalAlignment.Left;
                        SolidColorBrush background = new SolidColorBrush(Color.FromArgb(0,0,0,0));
                        SolidColorBrush forground = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));

                        if (lijst.ChatComments[i].FromUser_ID == MainViewVM.USER.ID)
                        {
                            aligment = HorizontalAlignment.Left;
                            background = new SolidColorBrush(Color.FromArgb(0, 100, 200, 200));
                            forground = new SolidColorBrush(Color.FromArgb(0, 200, 200, 200));
                        }
                        else
                        {
                            aligment = HorizontalAlignment.Right;
                            background = new SolidColorBrush(Color.FromArgb(0, 200, 200, 200));
                            forground = new SolidColorBrush(Color.FromArgb(0, 200, 200, 200));
                        }
                        lijst.ChatComments[i].Alignment = aligment;

                    }
                
                }
                else
                {
                    //geen comments
                }

                this.ChatRoom = lijst;
                RaiseAll();
            });

        }
    }
}
