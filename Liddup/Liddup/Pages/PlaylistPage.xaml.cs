using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Liddup.Models;
using Liddup.PageModels;

namespace Liddup.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlaylistPage : ContentView
    {
        public PlaylistPage(Playlist playlist)
        {
            BindingContext = new PlaylistPageModel(playlist);
            InitializeComponent();
        }
    }
}