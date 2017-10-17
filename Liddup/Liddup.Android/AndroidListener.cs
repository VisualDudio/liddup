using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Couchbase.Lite;
using Couchbase.Lite.Listener;
using Couchbase.Lite.Listener.Tcp;

namespace Liddup.Droid
{
    class AndroidListener : IListener
    {
        public AndroidListener() { }

        public void Start(Manager manager, ushort port)
        {
            var listener = new CouchbaseLiteTcpListener(manager, port, null);
        }
    }
}