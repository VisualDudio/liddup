using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Liddup
{
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private async void JoinButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MasterPlaylistPage(DependencyService.Get<INetworkManager>().GetDecryptedIPAddress(RoomCodeEntry.Text)));
        }

        private async void HostButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateRoomPage());
        }
    }
}
