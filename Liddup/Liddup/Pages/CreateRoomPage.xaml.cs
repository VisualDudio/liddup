﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Liddup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateRoomPage : ContentPage
    {
        public CreateRoomPage()
        {
            InitializeComponent();
        }

        private async void CreateRoomButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MasterPlaylistPage(DependencyService.Get<INetworkManager>().GetIPAddress()));
        }
    }
}