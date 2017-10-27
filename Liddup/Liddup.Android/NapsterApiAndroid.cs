using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Companion;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Com.Napster.Player;
using Com.Napster;
using Com.Napster.Cedar;
using Com.Napster.Cedar.Station;
using Com.Napster.Cedar.Player.Data;
using Com.Napster.Cedar.Player;
using Com.Napster.Cedar.Session;

using Xamarin.Forms;

namespace Liddup.Droid
{
    class NapsterApiAndroid
    {
        public Napster Napster { get; set; }
        
        public NapsterApiAndroid()
        {
            Napster = Napster.Register(Forms.Context, ApiConstants.NapsterApiKey, ApiConstants.NapsterApiSecret);
            
        }

        public void Login()
        {
            var loginUrl = Napster.GetLoginUrl(ApiConstants.NapsterRedirectUri);
        }
    }
}