﻿using BOBApp.Messages;
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
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BOBApp.ViewModels
{
    public class VindRitChatVM : ViewModelBase
    {
        //Properties
        public static int? ID;
       
        public string SearchLocation { get; set; }
        private Task task;
        public ChatRoom.All ChatRoom { get; set; }
        public RelayCommand AddCommentCommand { get; set; }
        public ChatComment ChatComment { get; set; }

       

        //Constructor
        public VindRitChatVM()
        {
            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);

            AddCommentCommand = new RelayCommand(AddComment);

            RaisePropertyChanged("ChatComment");
            getChatroom();
        }

        private void ExecuteNavigatedTo(NavigateTo obj)
        {
            if (obj.View == typeof(VindRitChat))
            {
                getChatroom();
            }
            if (obj.Reload == true)
            {
                getChatroom();
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




        //Methods
        private async void getChatroom()
        {
           
            try
            {

                if (VindRitChatVM.ID == null)
                {
                    string json = await Localdata.read("chatroom.json");
                    var definition = new { ID = 0 };
                    var data = JsonConvert.DeserializeAnonymousType(json, definition);
                    if (data.ID == -1)
                    {
                        //geen chatroom
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            Frame rootFrame = MainViewVM.MainFrame as Frame;
                            rootFrame.Navigate(typeof(VindRit));
                        });

                    }else
                    {
                        VindRitChatVM.ID = data.ID;
                        MainViewVM.ChatRoom = new ChatRoom() { ID = VindRitChatVM.ID.Value };

                        this.ChatComment = new ChatComment() { ChatRooms_ID = MainViewVM.ChatRoom.ID };
                        GetChatComments();

                     
                    }
                }
                else
                {
                    MainViewVM.ChatRoom = new ChatRoom() { ID = VindRitChatVM.ID.Value };

                    this.ChatComment = new ChatComment() { ChatRooms_ID = MainViewVM.ChatRoom.ID };
                    GetChatComments();


                }
                   

            }
            catch (Exception ex)
            {

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    Frame rootFrame = MainViewVM.MainFrame as Frame;
                    rootFrame.Navigate(typeof(VindRit));
                });
            }

           



            



        }


        private void AddComment()
        {
            //add comment to db
            task = AddComment_task();
        }

        private async Task<Boolean> AddComment_task()
        {
            Response res = await ChatRoomRepository.PostChatComment(this.ChatComment);
            if (res.Success == true)
            {

                int bobID=ChatRoom.ChatRoom.Bobs_ID;
                int userID = ChatRoom.ChatRoom.Users_ID;
                Libraries.Socket socketSendToBob = new Libraries.Socket()
                {
                    To = bobID,
                    Status = true
                };
                Libraries.Socket socketSendToUser = new Libraries.Socket()
                {
                    To = userID,
                    Status = true
                };

                MainViewVM.socket.Emit("chatroom_COMMENT:send", JsonConvert.SerializeObject(socketSendToUser));
                MainViewVM.socket.Emit("chatroom_COMMENT:send", JsonConvert.SerializeObject(socketSendToBob));


                this.ChatComment.Comment = "";
                RaisePropertyChanged("ChatComment");
               
            }
            return res.Success;
        }

        private async void GetChatComments()
        {
            ChatRoom.All lijst = await ChatRoomRepository.GetChatRoom(MainViewVM.ChatRoom.ID);

            if (lijst.ChatComments != null)
            {
                for (int i = 0; i < lijst.ChatComments.Count; i++)
                {
                    HorizontalAlignment aligment = HorizontalAlignment.Left; ;

                    if (lijst.ChatComments[i].FromUser_ID == MainViewVM.USER.ID)
                    {
                        aligment = HorizontalAlignment.Left;
                    }
                    else
                    {
                        aligment = HorizontalAlignment.Right;
                    }
                    lijst.ChatComments[i].Alignment = aligment;

                }
                
            }
            else
            {
                //geen comments
            }

            this.ChatRoom = lijst;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                RaisePropertyChanged("ChatRoom");
            });

        }
    }
}
