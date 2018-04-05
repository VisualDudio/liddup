using DLToolkit.Forms.Controls;
using Liddup.PageModels;
using Xamarin.Forms;

namespace Liddup
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            FlowListView.Init();
            Couchbase.Lite.Storage.SystemSQLite.Plugin.Register();

            MainPage = new FreshMvvm.FreshNavigationContainer(FreshMvvm.FreshPageModelResolver.ResolvePageModel<WelcomePageModel>());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
