using System.Windows.Input;
using Xamarin.Forms;
using Liddup.Models;
using System.Threading.Tasks;
using Liddup.Services;
using PropertyChanged;

namespace Liddup.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class CreateRoomPageModel : FreshMvvm.FreshBasePageModel
    {
        public bool IsBusy { get; set; }
        public bool ExplicitSongsAllowed { get; set; }
        public int SongRequestLimit { get; set; }
        public string Name { get; set; }

        ICommand _createRoomCommand;
        public ICommand CreateRoomCommand => _createRoomCommand ?? (_createRoomCommand = new Command(async () =>
        {
            IsBusy = true;
            await CreateRoom();
            IsBusy = false;
        }));

        public CreateRoomPageModel()
        {

        }

        private async Task CreateRoom()
        {
            var user = new User
            {
                IsHost = true
            };

            var room = new Room
            {
                ExplicitSongsAllowed = ExplicitSongsAllowed,
                SongRequestLimit = SongRequestLimit,
                Name = Name,
                Code = DependencyService.Get<INetworkManager>().GetEncryptedIPAddress(DependencyService.Get<INetworkManager>().GetIPAddress())
            };

            object[] data = { user, room };

            await CoreMethods.PushPageModel<AppContainerPageModel>(data, false);
        }
    }
}