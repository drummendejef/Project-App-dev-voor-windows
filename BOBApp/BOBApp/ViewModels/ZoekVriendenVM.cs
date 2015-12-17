using GalaSoft.MvvmLight;
using Libraries.Models;
using Libraries.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.ViewModels
{
    public class ZoekVriendenVM : ViewModelBase
    {
        //Properties

        //Constructor
        public ZoekVriendenVM()
        {
            AddFriend("Email");
        }

        private async void AddFriend(string email)
        {
            int userID = 0;//get bob from friend
            Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(userID)).Result;
            Libraries.Socket socketSend = new Libraries.Socket() { From = MainViewVM.USER.ID, To = bob.User.ID, Status = true };

            MainViewVM.socket.Emit("friend_REQUEST:send", JsonConvert.SerializeObject(socketSend));
        }

        //Methods


    }
}
