using Liddup.Models;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using FreshMvvm;

namespace Liddup.PageModels
{
    public class WelcomePageModel : FreshBasePageModel
    {
        public bool IsBusy { get; private set; }

        ICommand _joinRoomCommand; 
        public ICommand JoinRoomCommand => _joinRoomCommand ?? (_joinRoomCommand = new Command<string>(async s => await JoinRoom(s)));

        ICommand _createRoomCommand;
        public ICommand CreateRoomCommand => _createRoomCommand ?? (_createRoomCommand = new Command(async () => await CreateRoom()));

        private async Task JoinRoom(string roomCode)
        {
            var user = new User
            {
                IsHost = true
            };

            object[] data = { user, new Room() { Code = roomCode } };

            IsBusy = true;
            await CoreMethods.PushPageModelWithNewNavigation<AppContainerPageModel>(data, false);
        }

        private async Task CreateRoom()
        {
            await CoreMethods.PushPageModel<CreateRoomPageModel>(false);
        }
    }
}
