using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Libraries.Models;
using Libraries.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.ViewModels
{
    public class VindRitChatVM : ViewModelBase
    {
        //Properties
        public string SearchLocation { get; set; }
        private Task task;
        public ChatRoom.All ChatRoom { get; set; }
        public RelayCommand AddCommentCommand { get; set; }
        public ChatComment ChatComment { get; set; }

        //Constructor
        public VindRitChatVM()
        {
            AddCommentCommand = new RelayCommand(AddComment);
            //nieuwe chatroom virutaal aanmaken
            MainViewVM.ChatRoom = new ChatRoom() { ID = 1 };

            this.ChatComment = new ChatComment() { ChatRooms_ID = MainViewVM.ChatRoom.ID };
            GetChatComments();
        }



        //Methods
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
               
            }
            return res.Success;
        }

        private async void GetChatComments()
        {
            this.ChatRoom = await ChatRoomRepository.GetChatRoom(MainViewVM.ChatRoom.ID);
            RaisePropertyChanged("ChatRoom");
        }
    }
}
