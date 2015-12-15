using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Libraries;
using Libraries.Models;
using Libraries.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
           
            AddCommentCommand = new RelayCommand(AddComment);
           
            getChatroom();
        }




        //Methods
        private async void getChatroom()
        {
            if (VindRitChatVM.ID == null)
            {
                try
                {
                    string json = await Localdata.read("chatroom.json");
                    var definition = new { ID = 0 };
                    var data = JsonConvert.DeserializeAnonymousType(json, definition);
                    VindRitChatVM.ID = data.ID;
                }
                catch (Exception ex)
                {

                    Frame rootFrame = MainViewVM.MainFrame as Frame;
                    rootFrame.Navigate(typeof(VindRit));
                }

            }



            MainViewVM.ChatRoom = new ChatRoom() { ID = VindRitChatVM.ID.Value };

            this.ChatComment = new ChatComment() { ChatRooms_ID = MainViewVM.ChatRoom.ID };
            GetChatComments();



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
                GetChatComments();
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
            RaisePropertyChanged("ChatRoom");


        }
    }
}
